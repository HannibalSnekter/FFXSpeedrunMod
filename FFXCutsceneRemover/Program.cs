using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

internal sealed class CsrConfigBinder : BinderBase<CsrConfig>
{
    private readonly Option<bool?> _optCsrOn;
    private readonly Option<bool?> _optCsrBreakOn;
    private readonly Option<bool?> _optTrueRngOn;
    private readonly Option<bool?> _optSetSeedOn;
    private readonly Option<int?> _optMtSleepInterval;

    public CsrConfigBinder(Option<bool?> optCsrOn,
                           Option<bool?> optCsrBreakOn,
                           Option<bool?> optTrueRngOn,
                           Option<bool?> optSetSeedOn,
                           Option<int?> optMtSleepInterval)
    {
        _optCsrOn = optCsrOn;
        _optCsrBreakOn = optCsrBreakOn;
        _optTrueRngOn = optTrueRngOn;
        _optSetSeedOn = optSetSeedOn;
        _optMtSleepInterval = optMtSleepInterval;
    }

    private static bool ResolveMandatoryBoolArg(Option<bool?> opt)
    {
        Console.WriteLine(opt.Description);
        return Console.ReadLine()?.ToUpper().StartsWith("Y") ?? false;
    }

    protected override CsrConfig GetBoundValue(BindingContext bindingContext)
    {
        var csr_config = new CsrConfig {};

        csr_config.CsrOn = bindingContext.ParseResult.GetValueForOption(_optCsrOn) ?? ResolveMandatoryBoolArg(_optCsrOn);
        csr_config.CsrBreakOn = csr_config.CsrOn && ResolveMandatoryBoolArg(_optCsrBreakOn);

        csr_config.TrueRngOn = bindingContext.ParseResult.GetValueForOption(_optTrueRngOn) ?? ResolveMandatoryBoolArg(_optTrueRngOn);
        csr_config.SetSeedOn = !csr_config.TrueRngOn && ResolveMandatoryBoolArg(_optSetSeedOn);

        csr_config.MtSleepInterval = bindingContext.ParseResult.GetValueForOption(_optMtSleepInterval) ?? 16;

        return csr_config;
    }
}

internal sealed record CsrConfig
{
    public bool CsrOn { get; set; }
    public bool CsrBreakOn { get; set; }
    public bool TrueRngOn { get; set; }
    public bool SetSeedOn { get; set; }
    public int  MtSleepInterval { get; set; }
};

public class Program
{
    private static CsrConfig csrConfig;
    private static CutsceneRemover cutsceneRemover = null;
    private static RNGMod rngMod = null;

    private static Process Game = null;

    private static bool newGameSetUp = false;

    private static readonly BreakTransition BreakTransition = new BreakTransition { ForceLoad = false, Description = "Break Setup", ConsoleOutput = false, Suspendable = false, Repeatable = true };
	
    private static bool seedInjected = false;
    private static uint seedSubmitted;
    private static uint[] PCSeeds = new uint[] {
        2804382593,
        2807284884,
        2810252711,
        2813220538,
        2816122829,
        2819090656,
        2822058483,
        2825026310,
        2827928601,
        2830896428,
        2833864255,
        2836766546,
        2839734373,
        2842702200,
        2845670027,
        2848572318,
        2851540145,
        2854507972,
        2857410263,
        2860378090,
        2863345917,
        2866313744,
        2869216035,
        2872183862,
        2875151689,
        2878053980,
        2881021807,
        2883989634,
        2886957461,
        2889859752,
        2892827579,
        2895795406,
        2898697697,
        2901665524,
        2904633351,
        2907601178,
        2910503469,
        2913471296,
        2916439123,
        2919341414,
        2922309241,
        2925277068,
        2928244895,
        2931147186,
        2934115013,
        2937082840,
        2939985131,
        2942952958,
        2945920785,
        2948888612,
        2951790903,
        2954758730,
        2957726557,
        2960628848,
        2963596675,
        2966564502,
        2969532329,
        2972434620,
        2975402447,
        2978370274,
        2981272565,
        2984240392,
        2987208219,
        2990176046,
        2993078337,
        2996046164,
        2999013991,
        3001916282,
        3004884109,
        3007851936,
        3010819763,
        3013722054,
        3016689881,
        3019657708,
        3022559999,
        3025527826,
        3028495653,
        3031463480,
        3034365771,
        3037333598,
        3040301425,
        3043203716,
        3046171543,
        3049139370,
        3052107197,
        3055009488,
        3057977315,
        3060945142,
        3063847433,
        3066815260,
        3069783087,
        3072750914,
        3075653205,
        3078621032,
        3081588859,
        3084491150,
        3087458977,
        3090426804,
        3093394631,
        3096296922,
        3099264749,
        3102232576,
        3105134867,
        3108102694,
        3111070521,
        3114038348,
        3116940639,
        3119908466,
        3122876293,
        3125778584,
        3128746411,
        3131714238,
        3134682065,
        3137584356,
        3140552183,
        3143520010,
        3146422301,
        3149390128,
        3152357955,
        3155325782,
        3158228073,
        3161195900,
        3164163727,
        3167066018,
        3170033845,
        3173001672,
        3175969499,
        3178871790,
        3181839617,
        3184807444,
        3187709735,
        3190677562,
        3193645389,
        3196613216,
        3199515507,
        3202483334,
        3205451161,
        3208353452,
        3211321279,
        3214289106,
        3217256933,
        3220159224,
        3223127051,
        3226094878,
        3228997169,
        3231964996,
        3234932823,
        3237900650,
        3240802941,
        3243770768,
        3246738595,
        3249640886,
        3252608713,
        3255576540,
        3258544367,
        3261446658,
        3264414485,
        3267382312,
        3270284603,
        3273252430,
        3276220257,
        3279188084,
        3282090375,
        3285058202,
        3288026029,
        3290928320,
        3293896147,
        3296863974,
        3299831801,
        3302734092,
        3305701919,
        3308669746,
        3311572037,
        3314539864,
        3317507691,
        3320475518,
        3323377809,
        3326345636,
        3329313463,
        3332215754,
        3335183581,
        3338151408,
        3341119235,
        3344021526,
        3346989353,
        3349957180,
        3352859471,
        3355827298,
        3358795125,
        3361762952,
        3364665243,
        3367633070,
        3370600897,
        3373503188,
        3376471015,
        3379438842,
        3382406669,
        3385308960,
        3388276787,
        3391244614,
        3394146905,
        3397114732,
        3400082559,
        3403050386,
        3405952677,
        3408920504,
        3411888331,
        3414790622,
        3417758449,
        3420726276,
        3423694103,
        3426596394,
        3429564221,
        3432532048,
        3435434339,
        3438402166,
        3441369993,
        3444337820,
        3447240111,
        3450207938,
        3453175765,
        3456078056,
        3459045883,
        3462013710,
        3464981537,
        3467883828,
        3470851655,
        3473819482,
        3476721773,
        3479689600,
        3482657427,
        3485625254,
        3488527545,
        3491495372,
        3494463199,
        3497365490,
        3500333317,
        3503301144,
        3506268971,
        3509171262,
        3512139089,
        3515106916,
        3518009207,
        3520977034,
        3523944861,
        3526912688,
        3529814979,
        3532782806,
        3535750633,
        3538652924,
        3541620751,
        3544588578,
        3547556405,
        3550458696,
        3553426523,
        3556394350
    };

    // Cutscene Remover Version Number, 0x30 - 0x39 = 0 - 9, 0x48 = decimal point
    private const int majorID = 1;
    private const int minorID = 7;
    private const int patchID = 2;
    private static List<(string, byte)> startGameText;

    static Mutex mutex = new Mutex(true, "CSR");

    static void Main(string[] args)
    {
        if (CheckExistingCSR()) return;

        DiagnosticLog.Information($"Cutscene Remover for Final Fantasy X, version {majorID}.{minorID}.{patchID}");
        if (args.Length > 0) DiagnosticLog.Information($"!!! LAUNCHED WITH COMMAND-LINE OPTIONS: {string.Join(' ', args)} !!!");

        Option<bool?> optCsrOn           = new Option<bool?>("--csr", "Enable Cutscene Remover (CSR) Mod? [Y/N]");
        Option<bool?> optCsrBreakOn      = new Option<bool?>("--csrbreak", "Enable break for CSR? [Y/N]");
        Option<bool?> optTrueRngOn       = new Option<bool?>("--truerng", "Enable True RNG Mod? [Y/N]");
        Option<bool?> optSetSeedOn       = new Option<bool?>("--setseed", "Enable Set Seed Mod? [Y/N]");
        Option<int?>  optMtSleepInterval = new Option<int?>("--mt_sleep_interval", "Specify the main thread sleep interval. [ms]");

        RootCommand rootCmd = new RootCommand("Launches the FFX Cutscene Remover.")
        {
            optCsrOn,
            optCsrBreakOn,
            optTrueRngOn,
            optSetSeedOn,
            optMtSleepInterval
        };

        rootCmd.SetHandler(MainLoop, new CsrConfigBinder(optCsrOn, optCsrBreakOn, optTrueRngOn, optSetSeedOn, optMtSleepInterval));

        rootCmd.Invoke(args);
        return;
    }

    private static void MainLoop(CsrConfig config)
    {
        csrConfig = config;

        if (csrConfig.SetSeedOn)
        {
            SetSeed();
        }

        while (true)
        {
            Game = ConnectToTarget("FFX");

            if (Game == null)
            {
                continue;
            }

            MemoryWatchers.Initialize(Game);
            MemoryWatchers.Watchers.UpdateAll(Game);

            List<byte> startGameIndents = new List<byte> (8);

            byte language = MemoryWatchers.Language.Current;

            switch (language)
            {
                case 0x00: // Japanese
                    startGameIndents = new List<byte>() {
                        0x43,
                        0x00,
                        0x47,
                        0x43,
                        0x4b,
                        csrConfig.SetSeedOn ? (byte)0x48 : (byte)0x4b,
                        0x00,
                        0x4e
                    };
                    break;
                case 0x09: // Korean
                    startGameIndents = new List<byte>() {
                        0x43,
                        0x00,
                        0x46,
                        0x43,
                        0x4a,
                        csrConfig.SetSeedOn ? (byte)0x48 : (byte)0x4a,
                        0x00,
                        0x4d
                    };
                    break;
                case 0x0A: // Chinese
                    startGameIndents = new List<byte>() {
                        0x43,
                        0x00,
                        0x46,
                        0x43,
                        0x4a,
                        csrConfig.SetSeedOn ? (byte)0x46 : (byte)0x4a,
                        0x00,
                        0x4d
                    };
                    break;
                default:
                    startGameIndents = new List<byte>() {
                        0x43,
                        0x00,
                        0x45,
                        0x41,
                        0x4a,
                        csrConfig.SetSeedOn ? (byte)0x47 : (byte)0x4a,
                        0x00,
                        0x4d
                    };
                    break;
            }

            if (csrConfig.CsrOn)
            {
                cutsceneRemover = new CutsceneRemover(csrConfig.MtSleepInterval);
                cutsceneRemover.Game = Game;
            }

            if (csrConfig.TrueRngOn)
            {
                rngMod = new RNGMod();
                rngMod.Game = Game;
            }

            DiagnosticLog.Information("Starting main loop!");

            while (!Game.HasExited)
            {
                MemoryWatchers.Watchers.UpdateAll(Game);

                if (!newGameSetUp && MemoryWatchers.RoomNumber.Current == 0 && MemoryWatchers.Storyline.Current == 0 && MemoryWatchers.Dialogue1.Current == 6)
                {
                    if (csrConfig.SetSeedOn)
                    {
                        DiagnosticLog.Information($"Injecting Seed {seedSubmitted}");
                        new Transition { ForceLoad = false, SetSeed = true, SetSeedValue = unchecked((int)seedSubmitted), RoomNumberAlt = (short)(Array.IndexOf(PCSeeds, seedSubmitted) + 1) }.Execute();
                        seedInjected = true;
                    }

                    MemoryWatchers.Watchers.UpdateAll(Game);

                    startGameText = new List<(string, byte)>
                    {
                        ($"[FFX Speedrunning Mod v{majorID}.{minorID}.{patchID}]", startGameIndents[0]),
                        ($"", startGameIndents[1]),
                        ($"Cutscene Remover: {(csrConfig.CsrOn ? "Enabled" : "Disabled")}", startGameIndents[2]),
                        ($"Cutscene Remover Break: {(csrConfig.CsrBreakOn ? "Enabled" : "Disabled")}", startGameIndents[3]),
                        ($"True RNG: {(csrConfig.TrueRngOn ? "Enabled" : "Disabled")}", startGameIndents[4]),
                        ($"Set Seed: {(MemoryWatchers.RoomNumberAlt.Current != 0 ? PCSeeds[MemoryWatchers.RoomNumberAlt.Current - 1] : "Disabled")}", startGameIndents[5]),
                        ($"", startGameIndents[6]),
                        ($"Start Game?", startGameIndents[7])
                    };

                    new NewGameTransition { ForceLoad = false, ConsoleOutput = false, startGameText = startGameText }.Execute();

                    newGameSetUp = true;
                }
                if (newGameSetUp && MemoryWatchers.RoomNumber.Current == 23)
                {
                    newGameSetUp = false;
                }

                if (csrConfig.CsrBreakOn && MemoryWatchers.ForceLoad.Current == 0)
                {
                    if (MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1300)
                    {
                        new Transition { RoomNumber = 184, SpawnPoint = 0, Description = "Break" }.Execute();
                    }
                    else if (MemoryWatchers.RoomNumber.Current == 184 && MemoryWatchers.Storyline.Current == 1300)
                    {
                        BreakTransition.Execute();
                    }
                    else if (MemoryWatchers.RoomNumber.Current == 158 && MemoryWatchers.Storyline.Current == 1300)
                    {
                        new Transition { RoomNumber = 140, Storyline = 1310, SpawnPoint = 0, Description = "End of Break + Map + Rikku afraid + tutorial" }.Execute();
                    }
                }
                else
                {
                    if (MemoryWatchers.RoomNumber.Current == 140 && MemoryWatchers.Storyline.Current == 1300)
                    {
                        new Transition { RoomNumber = 140, Storyline = 1310, SpawnPoint = 0, Description = "End of Break + Map + Rikku afraid + tutorial" }.Execute();
                    }
				}

                if (csrConfig.CsrOn)
                {
                    cutsceneRemover.MainLoop();
                }

                if (csrConfig.TrueRngOn)
                {
                    rngMod.MainLoop();
                }

                // Sleep for a bit so we don't destroy CPUs
                Thread.Sleep(csrConfig.MtSleepInterval);
            }
        }
    }
    
    private static bool CheckExistingCSR()
    {
        bool isRunning = !mutex.WaitOne(TimeSpan.Zero, true);

        if (isRunning)
        {
            Console.WriteLine("Cutscene Remover is already running!");
            Console.ReadLine();
        }

        return isRunning;
    }

    private static void SetSeed()
    {
        bool seedEnteredByUser = false;

        while (!seedEnteredByUser)
        {
            Console.WriteLine("Enter Seed ID To Run");
            string seedString = Console.ReadLine();
            if (!uint.TryParse(seedString, out seedSubmitted))
            {
                Console.WriteLine("Seed Contained Non-Numeric Characters. Please Try Again.");
                continue;
            }
            if (!PCSeeds.Contains(seedSubmitted))
            {
                Console.WriteLine("Seed is not recognised as a valid PC Seed. Please Try Again.");
            }
            else
            {
                seedEnteredByUser = true;
            }
        }
    }

    private static Process ConnectToTarget(string TargetName)
    {
        Process Game = null;

        try
        {
            Game = Process.GetProcessesByName(TargetName).OrderByDescending(x => x.StartTime)
                     .FirstOrDefault(x => !x.HasExited);
        }
        catch (Win32Exception e)
        {
            DiagnosticLog.Information("Exception: " + e.Message);
        }

        if (Game == null || Game.HasExited)
        {
            Game = null;
            Console.Write("\rWaiting to connect to the game. Please launch the game if you haven't yet.");

            Thread.Sleep(500);
        }
        else
        {
            Console.Write("\n");
            DiagnosticLog.Information("Connected to FFX!");
        }

        return Game;
    }
}
