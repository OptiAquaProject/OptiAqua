using DatosOptiaqua;
using Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduledTasks {

    [DisallowConcurrentExecution]
    public class TareaQuartz : IJob {
#pragma warning disable CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica
        public async Task Execute(IJobExecutionContext context) {
#pragma warning restore CS1998 // El método asincrónico carece de operadores "await" y se ejecutará de forma sincrónica
            DB.InsertaEvento("Execute at " + DateTime.Now.ToString());
            CacheDatosHidricos.RecreateAll();
        }
    }

    public class JobScheduler {
        static JobKey jobKey;
        public static async Task Start(string cronExp) {
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
            if (!CronExpression.IsValidExpression(cronExp))
                return;
            IJobDetail job = JobBuilder.Create<TareaQuartz>().Build();
            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExp).Build();
            var tim = await scheduler.ScheduleJob(job, trigger);
            jobKey = job.Key;
        }

        public static async Task<bool> ChangeProgramation(string cronExp) {
            if (!CronExpression.IsValidExpression(cronExp))
                return false;

            IJobDetail job = JobBuilder.Create<TareaQuartz>().Build();
            IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            ITrigger trigger = TriggerBuilder.Create()
                .WithCronSchedule(cronExp)
                .Build();
            if (jobKey != null)
                await scheduler.DeleteJob(jobKey);
            var next = await scheduler.ScheduleJob(job, trigger);
            DB.InsertaEvento("Change prog next at:" + next.ToString());
            jobKey = job.Key;
            return next != null;
        }
    }
}
