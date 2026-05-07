using FFXCutsceneRemover.Logging;

namespace FFXCutsceneRemover;

class BikanelTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        if (MemoryWatchers.RoomNumber.Current == 136)
        {
            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                
                Stage += 1;

            }
            else if (MemoryWatchers.BikanelTransition.Current == (BaseCutsceneValue + 0x885D) && Stage == 1) // 11F
            {
                WriteValue<int>(MemoryWatchers.BikanelTransition, BaseCutsceneValue + 0x891A); // 1DC

                Transition actorPositions;
                // After the transition Kimahri's model is still visible so we bin him off to Narnia
                actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 4 }, Target_x = 1000.0f, Target_y = 0.0f, Target_z = -1000.0f };
                actorPositions.Execute();

                Stage += 1;
            }
        }
        else if (MemoryWatchers.RoomNumberAlt.Current == 136)
        {
            Stage = 0;
        }
    }
}