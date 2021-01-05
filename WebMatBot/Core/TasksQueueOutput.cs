using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebMatBot.Core
{
    public class TasksQueueOutput
    {
        public static int TimeSleeping { get; set; } = 0; // em segundos

        private static IList<Func<Task>> Queue = new List<Func<Task>>();

        public static async Task QueueAddSpeech(Func<Task> action, string user)
        {
            if (await SpeakerCore.CheckStatus(user))
                lock (Queue)
                    Queue.Add(action);
        }

        public static async Task QueueAddLight(Func<Task> action, string user)
        {
            if (await Lights.Light.CheckStatus(user))
            lock (Queue)
                Queue.Add(action);
        }

        public static async Task Start()
        {
            do
            {
                try
                {
                    if (SpeakerCore.State == SpeakerCore.Status.Disabled || SpeakerCore.State == SpeakerCore.Status.Paused)
                        await Task.Delay(2000);
                    else
                    {
                        Func<Task> scoped = null;

                        //get from list
                        lock (Queue)
                        {
                            if (Queue.Count > 0)
                            {
                                scoped = Queue[0];
                            }
                        }

                        //execute and wait
                        if (scoped != null)
                        {
                            await scoped();

                            //update list
                            lock (Queue)
                                Queue.Remove(Queue[0]);
                        }

                        await Task.Delay(TimeSleeping * 1000);
                    }
                }
                catch (Exception excpt)
                {
                    Console.WriteLine(excpt.Message);
                }
            } while (true);
        }

    }
}
