using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class DefenderXTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        int baseAddress = MemoryWatchers.GetBaseAddress();

        if (MemoryWatchers.RoomNumber.Current == 279)
        {
            if (Stage == 0 && MemoryWatchers.MovementLock.Current == 0x20)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage = 1;

            }
            else if (MemoryWatchers.DefenderXTransition.Current >= (BaseCutsceneValue + 0x5451) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.DefenderXTransition, BaseCutsceneValue + 0x586F);
                Stage = 2;
            }
        }
        else if (MemoryWatchers.RoomNumberAlt.Current == 279)
        {
            Stage = 0;
        }
    }
}