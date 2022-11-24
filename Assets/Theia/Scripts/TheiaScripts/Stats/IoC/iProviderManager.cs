﻿using Stats.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stats.IoC
{
    public interface iProviderManager<TProvider>
    {
        TProvider[] GetProviders();
    }
}
