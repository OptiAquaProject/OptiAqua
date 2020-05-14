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
        public async Task Execute(IJobExecutionContext context) {
            CacheDatosHidricos.RecreateAll(DateTime.Now.Date);
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
            var tim = await scheduler.ScheduleJob(job, trigger);
            jobKey = job.Key;
            return tim != null;
        }
    }
}
