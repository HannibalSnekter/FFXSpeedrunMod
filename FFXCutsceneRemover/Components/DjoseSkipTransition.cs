namespace FFXCutsceneRemover;

class DjoseSkipTransition : Transition
{
    bool skip_initiated = false;
    bool skip_failed    = false;

    public override void Execute(string defaultDescription = "")
    {
        bool precondition = MemoryWatchers.RoomNumber.Current == 93 &&
                            MemoryWatchers.Storyline.Current  == 961;

        if (precondition && MemoryWatchers.NPCLastInteraction.Current == 12 && !skip_failed)
        {
            skip_initiated = true;
        }

        if (skip_initiated && (MemoryWatchers.AtelCurCtrlWorker.Current & 0x02) == 0x02) // The user regained control before we skipped; failure
        {
            skip_failed = true;
        }

        if (precondition && MemoryWatchers.CutsceneAlt.Current == 1678)
        {
            bool skip = skip_initiated && !skip_failed;

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
