using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using Framework;
    using System;
    using System.Collections;
    using Action = Core.Actions.Action;

    public static class Extensions
    {
        #region -------------------------------- Strings and regex --------------------------------

        private static readonly Regex k_Whitespace = new Regex(@"\s+");

        private static readonly string[] k_Keywords = new[]
        {
            "bool", "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "double", "float", "decimal",
            "string", "char", "void", "object", "typeof", "sizeof", "null", "true", "false", "if", "else", "while", "for", "foreach", "do", "switch",
            "case", "default", "lock", "try", "throw", "catch", "finally", "goto", "break", "continue", "return", "public", "private", "internal",
            "protected", "static", "readonly", "sealed", "const", "fixed", "stackalloc", "volatile", "new", "override", "abstract", "virtual",
            "event", "extern", "ref", "out", "in", "is", "as", "params", "__arglist", "__makeref", "__reftype", "__refvalue", "this", "base",
            "namespace", "using", "class", "struct", "interface", "enum", "delegate", "checked", "unchecked", "unsafe", "operator", "implicit", "explicit"
        };

        public static string CamelCaseToSpaced(this string input)
        {
            return Regex.Replace(input, "([A-Z])", " $1").Trim();
        }

        public static string RemoveWhitespaces(this string str)
        {
            return k_Whitespace.Replace(str, "");
        }

        public static string RemovePunctuationsAndSymbols(this string str)
        {
            return string.Concat(str.Where(c => !char.IsWhiteSpace(c) && !char.IsPunctuation(c) && !char.IsSymbol(c)));
        }

        public static string Join(this IEnumerable<string> strings, string separator = ", ") => string.Join(separator, strings);



        public static string RemoveTermination(this string str, string termination)
        {
            if (str.EndsWith(termination))
            {
                str = str.Substring(0, str.Length - termination.Length);
            }
            return str;
        }

        public static string TypeName(this object obj) => obj.GetType().Name;
        public static string ToCodeFormat(this float f) => f.ToString().Replace(',', '.') + "f";
        public static string ToCodeFormat(this bool b) => b.ToString().ToLower();
        public static string ToCodeFormat(this Status s) => "Status." + s.ToString();
        public static string ToCodeFormat(this StatusFlags s) => "StatusFlags." + ((int)s < 0 ? StatusFlags.Active.ToString() : s.ToString());
        public static string ToCodeFormat(this ExecutionInterruptOptions s) => "ExecutionInterruptOptions." + s.ToString();

        public static string DisplayInfo(this StatusFlags statusFlags)
        {
            List<string> raisedFlags = new List<string>();
            if ((statusFlags & StatusFlags.Running) == StatusFlags.Running) raisedFlags.Add("Running");
            if ((statusFlags & StatusFlags.Success) == StatusFlags.Success) raisedFlags.Add("Success");
            if ((statusFlags & StatusFlags.Failure) == StatusFlags.Failure) raisedFlags.Add("Failure");

            if(raisedFlags.Count == 0)
            {
                return "disabled";
            }
            else if(raisedFlags.Count == 3)
            {
                return "always";
            }
            else
            {
                return string.Join(" | ", raisedFlags);
            }
        }

        public static string GetInfo(this ReferenceData referenceData) 
        {
            var graphMap = new Dictionary<string, string>();

            if(!BehaviourSystemEditorWindow.instance.IsRuntime)
            {
                graphMap = BehaviourSystemEditorWindow.instance.System.Data.graphs.ToDictionary(g => g.id, g => g.name);
            }

            if (referenceData.Value != null)
            {
                string info = referenceData.Value.ToString();

                if (info.StartsWith('@'))
                {
                    string pattern = @"_g\(([a-f0-9\-]*)\)";
                    return Regex.Replace(info.Substring(1), pattern, Replacement);
                }
                else
                {
                    return info;
                }
            }
            else
            {
                return "-";
            }

            string Replacement(Match match)
            {
                string key = match.Groups[1].Value;
                if (graphMap.ContainsKey(key))
                {
                    return graphMap[key];
                }
                return match.Value;
            }
        }

        public static void MoveAtFirst<T>(this List<T> list, T element)
        {
            if (list.Remove(element)) list.Insert(0, element);
        }

        #endregion
    }
}
