// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Net;
using CommandLine;

//parsing of command line parameters
var result = Parser.Default.ParseArguments<Options>(args)
    .WithParsed(RunOptions);

//function for extract data from log then write to output file
static void RunOptions(Options opts) {
    if(!File.Exists(opts.file_log)) {
        Console.WriteLine("Can't find file_log");
        return;
    }

    if(!File.Exists(opts.file_output)) {
        Console.WriteLine("Can't find file_output");
        return;
    }
    
    ICheck checker;
    if(opts.address_start == null) {
        checker = new DataChecker(opts.time_start, opts.time_end);
    } else {
        checker = new IPandDataChecker(opts.address_start, opts.address_mask,
            opts.time_start, opts.time_end);
    }

    if(!checker.IsValid()) {
        checker.PrintStatus();
        return;
    }

    Dictionary<string, int> ip_count = new();

    string? log_line = "";

    try {

    using(StreamReader sr = new(opts.file_log))
    while ((log_line = sr.ReadLine()) != null) {
        string[] ipanddate = log_line.Split(new char[] {':'}, 2);
        string ip = ipanddate[0].Trim();
        string date = ipanddate[1].Trim();
        if(checker.Check(ip, date)) {
            if(ip_count.ContainsKey(ip)) {
                ip_count[ip]++;
            } else {
                ip_count[ip] = 1;
            }
        }
    }

    using(StreamWriter sw = new(opts.file_output))
    foreach(var pair in ip_count) {
        sw.WriteLine($"{pair.Key} {pair.Value}");
    }

    Console.WriteLine($"Successful");
    
    } catch(Exception e) {
        Console.WriteLine($"Process failed: " + e.ToString());
    }
    
}


//Command line parameters
class Options {
    [Option("file-log", Required = true)]
    public string? file_log {get;set;}
    [Option("file-output", Required = true)]
    public string? file_output {get;set;}
    [Option("address-start")]
    public string? address_start {get;set;}
    [Option("address-mask")]
    public string? address_mask {get;set;}
    [Option("time-start", Required = true)]
    public string? time_start {get;set;}
    [Option("time-end", Required = true)]
    public string? time_end {get;set;}
    
}


//This interface is used to parse and validate initial command line parameters ip and date
//and although to filter certain ip and date from log file for count
interface ICheck {
    //checking if ip and date is needed to count
    bool Check(string ip, string date);
    //checking if command line parameters ip and date are valid
    bool IsValid();
    //print if command line parameters ip and date are valid
    void PrintStatus();
}

//This class is used for checking data from log file 
//when parameters address_start and address_mask is not used by user
class DataChecker : ICheck {
    DateTime dateTime_start;
    DateTime dateTime_end;
    DateTime dateTime_cur;
    bool start_valid;
    bool end_valid;
    bool cur_valid;
    public DataChecker(string? time_start, string? time_end) {
        start_valid = DateTime.TryParseExact(time_start, 
                    "dd.MM.yyyy", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out dateTime_start);
        end_valid = DateTime.TryParseExact(time_end,
                    "dd.MM.yyyy", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, 
                    out dateTime_end);
    }  
    public virtual bool Check(string ip, string date) {
        return check_date_boundaries(date);
    }

    public virtual bool IsValid() {
        return start_valid && end_valid;
    }

    public virtual void PrintStatus() {
        Console.WriteLine($"Correctness of time_start is {start_valid}");
        Console.WriteLine($"Correctness of time_end is {end_valid}");
    }

    bool check_date_boundaries(string cur) {
        cur_valid = DateTime.TryParseExact(cur,
                    "yyyy-MM-dd HH:mm:ss", 
                    CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out dateTime_cur);
        return cur_valid && 
            (dateTime_start <= dateTime_cur) && (dateTime_cur <= dateTime_end);
    }
}

//This class is used for checking data and ip from log file 
//when parameters address_start and address_mask is used by user.
//Function IpTryParse is needed to convert string representation of ipv4 to uint type
//for ease checking if ip from log file is apropriate to count
class IPandDataChecker : DataChecker {
    uint uint_start;
    uint uint_mask;
    uint uint_cur;
    bool start_valid;
    bool mask_valid;
    bool cur_valid;

    public IPandDataChecker(string? address_start, string? address_mask,
        string? time_start, string? time_end) 
        : base(time_start, time_end) {
        start_valid = IpTryParse(address_start, out uint_start);

        if(address_mask != null)
            mask_valid = IpTryParse(address_mask, out uint_mask);
        else {
            uint_mask = 0;
            mask_valid = true;
        }

    }
    public override bool Check(string ip, string date) {
        return base.Check(ip, date) && check_ip_boundaries(ip);
    }

    public override bool IsValid() {
        return base.IsValid()
            && start_valid && mask_valid;
    }

    public override void PrintStatus() {
        base.PrintStatus();
        Console.WriteLine($"Correctness of address_start is {start_valid}");
        Console.WriteLine($"Correctness of address_mask is {mask_valid}");
    }

    
    static bool IpTryParse(string? ip_string, out uint ip_int) {
        ip_int = 0;
        IPAddress? ip_address;
        if(!IPAddress.TryParse(ip_string, out ip_address))
            return false;
    
        byte[] ip_bytes = ip_address.GetAddressBytes();
    
        if(BitConverter.IsLittleEndian) {
            Array.Reverse(ip_bytes);
        }

        ip_int = BitConverter.ToUInt32(ip_bytes, 0);
        return true;
    }

    bool check_ip_boundaries(string ip_cur) {
        cur_valid = IpTryParse(ip_cur, out uint_cur);
        return cur_valid 
        && (uint_cur >= uint_start)
        && ((uint_cur & uint_mask) == (uint_start & uint_mask));
    }
}