using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Simpbot.Core.Persistence.Entity;

namespace Simpbot.Core.Persistence.Seed
{
    public class PrefixSeed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetService<PrefixContext>())
            {
               if (context.PrefixDefaults.Any())
                    return;

                context.PrefixDefaults.Add(
                    new PrefixDefault
                    {
                        PrefixSymbol = '.'
                    }
                );
                var result = context.SaveChanges();
            }
        }
    }
}