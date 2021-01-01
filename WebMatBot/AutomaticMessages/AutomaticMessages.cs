using Google.Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static WebMatBot.ScheduledMessage;

namespace WebMatBot
{
    public class AutomaticMessages
    {
        public static DateTime LastMessage { get; set; } = DateTime.Now.AddMinutes(-5);
        private static TimeSpan SpaceBetweenMessages = new TimeSpan(0,30,0);

        private static List<ScheduledMessage> ScheduledQueue = new List<ScheduledMessage>();
        private static List<ScheduledMessage> LiquidQueue = new List<ScheduledMessage>();


        public static async Task StartScheduled()
        {
            //inicialização tempo
            await Task.Delay(new TimeSpan(0, 1, 0));

            lock (ScheduledQueue)
            {
                ScheduledQueue.Add(DrinkWater(DateTime.Now));
                ScheduledQueue.Add(Discord(DateTime.Now));
                ScheduledQueue.Add(Youtube(DateTime.Now));
                ScheduledQueue.Add(GitHub(DateTime.Now));
                ScheduledQueue.Add(Donate(DateTime.Now));
                //Queue.Add(Form(DateTime.Now));
            }


            do
            {
                ScheduledMessage Item;

                lock (ScheduledQueue)
                    Item = ScheduledQueue.OrderBy(q => q.DateSchedule).FirstOrDefault(item => item.DateSchedule <= DateTime.Now && DateTime.Now >= LastMessage.Add(SpaceBetweenMessages));

                if (Item != null)
                    Item.Action.Invoke(Item);

                await Task.Delay(20000);
            }
            while (true);
        }

        public static async Task StartLiquid()
        {
            //inicialização tempo
            await Task.Delay(new TimeSpan(0, 45, 0));

            lock (LiquidQueue)
            {
                LiquidQueue.Add(Commercial(DateTime.Now));
            }


            do
            {
                ScheduledMessage Item;

                lock (LiquidQueue)
                    Item = LiquidQueue.OrderBy(q => q.DateSchedule).FirstOrDefault(item => item.DateSchedule <= DateTime.Now);

                if (Item != null)
                    Item.Action.Invoke(Item);

                await Task.Delay(20000);
            }
            while (true);
        }

        public static void AddScheduledQueue(ScheduledMessage Item)
        {
            lock (ScheduledQueue)
                ScheduledQueue.Add(Item);
        }

        public static void RemoveScheduledQueue(MessageType type)
        {
            ScheduledMessage item;
            lock (ScheduledQueue)
                item = ScheduledQueue.FirstOrDefault(q => q.TypeInfo == type);

            if (item != null)
                ScheduledQueue.Remove(item);
        }

        public static void AddLiquidQueue(ScheduledMessage Item)
        {
            lock (ScheduledQueue)
                LiquidQueue.Add(Item);
        }

        public static void RemoveLiquidQueue(MessageType type)
        {
            ScheduledMessage item;
            lock (LiquidQueue)
                item = LiquidQueue.FirstOrDefault(q => q.TypeInfo == type);

            if (item != null)
                LiquidQueue.Remove(item);
        }
    }
}
