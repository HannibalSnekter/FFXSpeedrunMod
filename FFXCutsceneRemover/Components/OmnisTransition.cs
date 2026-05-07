using System.Collections.Generic;

namespace FFXCutsceneRemover;

class OmnisTransition : Transition
{
    static private List<short> CutsceneAltList = new List<short>(new short[] { 5331 });
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.RoomNumber.Current == 296)
        {
            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0x1DB0) && Stage == 1)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0x1E2C);

                Stage += 1;
            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0x1E3B) && Stage == 2)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0x25B5);

                Stage += 1;
            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0x25BB) && Stage == 3)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0x274A);

                Stage += 1;
            }
            else if (MemoryWatchers.OmnisTransition.Current >= (BaseCutsceneValue + 0x2786) && MemoryWatchers.BattleState2.Current == 22 && Stage == 4)
            {
                WriteValue<int>(MemoryWatchers.OmnisTransition, BaseCutsceneValue + 0x2D84);

                Stage += 1;
            }
        }
        else if (MemoryWatchers.RoomNumberAlt.Current == 296)
        {
            Stage = 0;
        }
    }
}