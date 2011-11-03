﻿// Copyright (C) 2011, Mehdi Khalili
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the <organization> nor the
//       names of its contributors may be used to endorse or promote products
//       derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bddify.Core;

namespace Bddify.Module
{
    public class ModuleFactory
    {
        // cache modules for performance reason
        private static readonly IEnumerable<IModule> Modules;

        static ModuleFactory()
        {
            Modules = GetModules();
        }

        public static T Get<T>(Story story) where T:IModule
        {
            return Modules.OfType<T>().First(c => c.RunsOn(story));
        }

        public static IEnumerable<T> GetMany<T>(Story story) where T:IModule
        {
            return Modules.OfType<T>().Where(c => c.RunsOn(story));
        }

        private static IEnumerable<IModule> GetModules()
        {
#if !SILVERLIGHT
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.FullName.StartsWith("System")).SelectMany(a => a.GetTypes());
            return GetModules(types).OrderBy(c => c.Priority);
#else
            // ToDo: need to fix this
            yield break;
#endif
        }

        private static IEnumerable<IModule> GetModules(IEnumerable<Type> types)
        {
            return from type in types
                    where type.IsClass && !type.IsAbstract && typeof(IModule).IsAssignableFrom(type) // the configs in the test assembly
                    select (IModule)Activator.CreateInstance(type);
        }
    }
}