using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using HarmonyLib;

public class TimeFreeze : IModApi
{
    public void InitMod(Mod mod)
    {
        new Harmony(base.GetType().ToString()).PatchAll(Assembly.GetExecutingAssembly());
    }

    public TimeFreeze()
    {
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
        {
            Timer timer = new Timer(this.interval);
            timer.Elapsed += this.OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }
    }


    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    private void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini"))
            {
                this.ReadAllIni();
            }
            else
            {
                this.CreateIni();
            }
            if (GameStats.GetInt(EnumGameStats.GameState) == 1)
            {
                if (this.time_reset_enable)
                {
                   dotimesave = false;
                    if (!dotimesave)
                    {
                        try
                        {
                            new ConsoleCmdSetTime().Execute(new List<string>
                            {
                                this.time_day,
                                this.time_hour,
                                this.time_minute
                            }, default(CommandSenderInfo));
                        }
                        catch
                        {
                        }
                        finally
                        {
                            dotimesave = true;
                        }
                    }
                }
                if (this.weather_reset_enable)
                {
                    dotimesave = false;
                    if (!dotimesave)
                    {
                        try
                        {
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "clouds",
                                this.weather_clouds
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "rain",
                                this.weather_rain
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "wet",
                                this.weather_wet
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "snow",
                                this.weather_snow
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "snowfall",
                                this.weather_snowfall
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "wind",
                                this.weather_wind
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "fog",
                                this.weather_fog
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "fogcolor",
                                this.weather_fogcolor[0],
                                this.weather_fogcolor[1],
                                this.weather_fogcolor[2]
                            }, default(CommandSenderInfo));
                            new ConsoleCmdWeather().Execute(new List<string>
                            {
                                "temp",
                                this.weather_temp
                            }, default(CommandSenderInfo));
                        }
                        catch
                        {
                        }
                        finally
                        {
                            dotimesave = true;
                        }
                    }
                }
            }
        }
    }
    public void ReadAllIni()
    {
        this.interval = Convert.ToDouble(this.ReadIni("general", "interval"));
        this.time_reset_enable = Convert.ToBoolean(this.ReadIni("time", "reset_enable"));
        this.weather_reset_enable = Convert.ToBoolean(this.ReadIni("weather", "reset_enable"));
        this.time_day = this.ReadIni("time", "day");
        this.time_hour = this.ReadIni("time", "hour");
        this.time_minute = this.ReadIni("time", "minute");
        this.weather_clouds = this.ReadIni("weather", "clouds");
        this.weather_rain = this.ReadIni("weather", "rain");
        this.weather_wet = this.ReadIni("weather", "wet");
        this.weather_snow = this.ReadIni("weather", "snow");
        this.weather_snowfall = this.ReadIni("weather", "snowfall");
        this.weather_wind = this.ReadIni("weather", "wind");
        this.weather_fog = this.ReadIni("weather", "fog");
        this.weather_temp = this.ReadIni("weather", "temp");
        this.weather_fogcolor = this.ReadIni("weather", "fogcolor").Split(',');
    }

    public string ReadIni(string _section, string _key)
    {
        StringBuilder stringBuilder = new StringBuilder(255);
        GetPrivateProfileString(_section, _key, "", stringBuilder, 255, Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        return stringBuilder.ToString();
    }

    public void CreateIni()
    {
        File.WriteAllText(Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini", "[general]interval=200\r\n[time]\r\nreset_enable=true\r\nday=1\r\nhour=12\r\nminute=00\r\n[weather]\r\nreset_enable=true\r\nclouds=0\r\nrain=0\r\nwet=0\r\nsnow=0\r\nsnowfall=0\r\nwind=0\r\nfog=0.02\r\nfogcolor=0.47,0.51,0.55\r\ntemp=70");
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    public string time_day = "1";
    public string time_hour = "12";
    public string time_minute = "00";
    public string weather_clouds = "0";
    public string weather_rain = "0";
    public string weather_wet = "0";
    public string weather_snow = "0";
    public string weather_snowfall = "0";
    public string weather_wind = "0";
    public string weather_fog = "0.02";
    public string[] weather_fogcolor = new string[]
    {
        "0.47",
        "0.51",
        "0.55"
    };
    public bool time_reset_enable = true;
    public bool weather_reset_enable = true;
    public double interval = 200.0;
    public static bool dotimesave = true;
    public string weather_temp = "70";

    [HarmonyPatch(typeof(ConsoleCmdSetTime))]
    [HarmonyPatch("Execute")]
    public class ConsoleCmdSetTime_Execute
    {
        public static bool Prefix(List<string> _params)
        {
            try
            {
                if (_params.Count != 0 && dotimesave)
                {
               
                    if (_params[0] == "off")
                    {
                        WritePrivateProfileString("time", "reset_enable", "false", Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
                        new ConsoleCmdServerMessage().Execute(new List<string> { "Time freeze have been disabled!" }, default(CommandSenderInfo));
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Time freeze have been disabled!");
                        return false;

                    }
                    else if (_params[0] == "on")
                    {
                            if (_params.Count == 2)
                            {
                                if (_params[0] == "on" && _params[1] == "now")
                                {
                                    string[] currentime = ValueDisplayFormatters.WorldTime(GameManager.Instance.World.worldTime, "{0} {1} {2}").Split(' ');
                                    WriteTimeToIni(currentime[0], currentime[1], currentime[2]);
                                }
                            }
                 

                        WritePrivateProfileString("time", "reset_enable", "true", Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
                        new ConsoleCmdServerMessage().Execute(new List<string> {  "Time freeze have been enabled!" }, default(CommandSenderInfo));
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Time freeze have been enabled!");
                        return false;
                    }
                    else
                    {
                        double output = 0;
                        if (double.TryParse(_params[0], out output) && double.TryParse(_params[1], out output) && double.TryParse(_params[2], out output))
                        {
                            WriteTimeToIni(_params[0], _params[1], _params[2]);
                        }
                    }
                    
                }
            }
            catch
            {
            }

            return true;
        }
    }

    public static void WriteTimeToIni(string _day, string _hour, string _minute)
    {
        WritePrivateProfileString("time", "day", _day, Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        WritePrivateProfileString("time", "hour", _hour, Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        WritePrivateProfileString("time", "minute", _minute, Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
    }

    [HarmonyPatch(typeof(ConsoleCmdWeather))]
    [HarmonyPatch("Execute")]
    public class ConsoleCmdWeather_Execute
    {
        public static bool Prefix(List<string> _params)
        {
            try
            {
                if (_params.Count != 0 && dotimesave)
                {
                    if (_params[0] == "off")
                    {
                        WritePrivateProfileString("weather", "reset_enable", "false", Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
                        new ConsoleCmdServerMessage().Execute(new List<string>
                        {
                            "Weather freeze have been disabled!"
                        }, default(CommandSenderInfo));
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Weather freeze have been disabled!");
                        return false;
                    }
                    else if (_params[0] == "on")
                    {
                        WritePrivateProfileString("weather", "reset_enable", "true", Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
                        new ConsoleCmdServerMessage().Execute(new List<string>
                        {
                            "Weather freeze have been enabled!"
                        }, default(CommandSenderInfo));
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Weather freeze have been enabled!");
                        return false;
                    }
                    else
                    {
                        WriteWeatherToini(_params);
                    }
                }
            }
            catch
            {
            }

            return true;
        }
    }

    public static void WriteWeatherToini(List<string> _params)
    {
        if (_params[0] == "clouds")
        {
            WritePrivateProfileString("weather", "clouds", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "rain")
        {
            WritePrivateProfileString("weather", "rain", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "wet")
        {
            WritePrivateProfileString("weather", "wet", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "snow")
        {
            WritePrivateProfileString("weather", "snow", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "snowfall")
        {
            WritePrivateProfileString("weather", "snowfall", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "wind")
        {
            WritePrivateProfileString("weather", "wind", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "fog")
        {
            WritePrivateProfileString("weather", "fog", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "fogcolor")
        {
            WritePrivateProfileString("weather", "fogcolor", string.Concat(new string[]
            {
                            _params[1],
                            ",",
                            _params[2],
                            ",",
                            _params[3]
            }), Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
        else if (_params[0] == "temp" || _params[0] == "temperature")
        {
            WritePrivateProfileString("weather", "temp", _params[1], Environment.CurrentDirectory + "\\Mods\\TimeFreeze\\settings.ini");
        }
    }
}
