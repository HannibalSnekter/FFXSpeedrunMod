using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class DjoseSkipTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.NPCLastInteraction.Current == 12 && Stage == 0)
        {
            DiagnosticLog.Information("Djose Skip initiated.");
            Stage = 1;

            return;
        }

        if (Stage == 1 && (MemoryWatchers.AtelCurCtrlWorker.Current & 0x02) == 0x02) // The user regained control before we skipped; failure
        {
            DiagnosticLog.Information("Djose Skip failed. You regained control over your character before completing the skip.");
            Stage = 2;

            return;
        }

        if (MemoryWatchers.CutsceneAlt.Current == 1678)
        {
            if (Stage == 1)
            {
                Stage = 3; // Success
                return;
            }

            if (Stage != 3) // Failure, or not attempted
            {
                Transition t = new()
                {
                    RoomNumber  = 76,
                    Storyline   = 962,
                    SpawnPoint  = 1,
                    Description = "Tidus is eager to go to Zanarkand"
                };

                t.Execute();
            }
        }
    }
}
