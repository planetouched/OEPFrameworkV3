﻿using System;
using System.Collections.Generic;
using OEPFrameworkV3._Base;

namespace OEPFrameworkV3.Utils
{
    public static class GEvent
    {
        private static int _uniqueCounter;

        public static string GetUniqueCategory()
        {
            return "#" + _uniqueCounter++;
        }

        private static readonly Dictionary<string, List<(bool global, Action<object> action)>> _actions = new();

        public static void Attach(string category, Action<object> method, IDestroyableObject puller, bool global = false)
        {
            if (_actions.TryGetValue(category, out var list))
            {
                list.Add((global, method));
            }
            else
            {
                _actions.Add(category, new List<(bool global, Action<object> action)> {(global, method)});
            }

            if (puller != null)
            {
                puller.Destroyed += _ => { Detach(category, method); };
            }
        }

        public static void Detach(string category, Action<object> method, bool global = false)
        {
            if (!_actions.TryGetValue(category, out var list))
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].global == global && list[i].action == method)
                {
                    list.RemoveAt(i);
                }
            }

            if (list.Count == 0)
            {
                _actions.Remove(category);
            }
        }

        public static void DetachFromLocalSubscriptions()
        {
            foreach (var pair in _actions)
            {
                for (var i = pair.Value.Count - 1; i >= 0; i--)
                {
                    if (!pair.Value[i].global)
                    {
                        pair.Value.RemoveAt(i);
                    }
                }
            }
        }

        public static void Call(string category, object obj = null)
        {
            if (!_actions.TryGetValue(category, out var list))
            {
                return;
            }

            if (list.Count == 0)
            {
                return;
            }

            switch (list.Count)
            {
                case 1:
                {
                    list[0].action(obj);
                    break;
                }
                case 2:
                {
                    var tmp0 = list[0].action;
                    var tmp1 = list[1].action;
                    tmp0(obj);
                    tmp1(obj);
                    break;
                }
                case 3:
                {
                    var tmp0 = list[0].action;
                    var tmp1 = list[1].action;
                    var tmp2 = list[2].action;
                    tmp0(obj);
                    tmp1(obj);
                    tmp2(obj);
                    break;
                }
                default:
                {
                    var copy = new (bool global, Action<object> action)[list.Count];
                    list.CopyTo(copy, 0);
                    foreach (var pair in copy)
                        pair.action(obj);
                    break;
                }
            }
        }

        public static void DetachCategory(string category)
        {
            _actions.Remove(category);
        }
    }
}