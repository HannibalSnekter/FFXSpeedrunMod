using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class DjoseSkipTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        bool precondition = MemoryWatchers.RoomNumber.Current == 93 &&
                            MemoryWatchers.Storyline .Current == 961;

        if (precondition && MemoryWatchers.NPCLastInteraction.Current == 12 && Stage == 0)
        {
            DiagnosticLog.Information("Djose Skip initiated.");
            Stage = 1;

            return;
        }

        if (Stage == 1 && (MemoryWatchers.AtelCurCtrlWorker.Current & 0x02) == 0x02) // The user regained control before we skipped; failure
        {
            DiagnosticLog.Information("Djose Skip failed. You regained control over your character before we skipped.");
            Stage = 2;

            return;
        }

        if (precondition && MemoryWatchers.CutsceneAlt.Current == 1678)
        {
            bool skip = Stage == 1;

            Transition t = new()
            {
                RoomNumber  = (short?) (skip ? 75   : 76),
                Storyline   = (short?) (skip ? 1030 : 962),
                SpawnPoint  = (byte?)  (skip ? 0    : 1),
                Description = skip
                    ? "Djose Skip? In THIS economy?"
                    : "Tidus is eager to go to Zanarkand"
            };

            t.Execute();
        }
    }
}
