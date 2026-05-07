using FFXCutsceneRemover.ComponentUtil;
using FFXCutsceneRemover.Logging;
using System.Diagnostics;

namespace FFXCutsceneRemover;

class ChocoboEaterTransition : Transition
{
    public override void Execute(string defaultDescription = "")
    {
        Process process = MemoryWatchers.Process;

        if (MemoryWatchers.RoomNumber.Current == 58)
        {

            if (MemoryWatchers.MovementLock.Current == 0x20 && Stage == 0)
            {
                base.Execute();

                BaseCutsceneValue = MemoryWatchers.EventFileStart.Current;
                Stage += 1;

            }
            else if (MemoryWatchers.ChocoboEaterTransition.Current >= (BaseCutsceneValue + 0xCF04) && Stage == 1) // 7A5
            {
                WriteValue<int>(MemoryWatchers.ChocoboEaterTransition, BaseCutsceneValue + 0xD3F5);// 30A

                byte[] ActiveParty = process.ReadBytes(MemoryWatchers.Formation.Address, 3);

                for (int i = 0; i < 3; i++)
                {
                    if (ActiveParty[i] == 0x00)
                    {
                        Transition actorPositions;
                        //Position Tidus if he is in the party
                        actorPositions = new Transition { ForceLoad = false, ConsoleOutput = false, TargetActorIDs = new short[] { 1 }, Target_x = 100.0f, Target_y = 0.0f, Target_z = 30.0f };
                        actorPositions.Execute();
                    }
                }

                Stage += 1;
            }
        }
        else if (MemoryWatchers.RoomNumberAlt.Current == 58)
        {
            Stage = 0;
        }
    }
}