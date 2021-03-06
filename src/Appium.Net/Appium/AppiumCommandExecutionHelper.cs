﻿//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//See the NOTICE file distributed with this work for additional
//information regarding copyright ownership.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using OpenQA.Selenium.Appium.Interfaces;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace OpenQA.Selenium.Appium
{
    public class AppiumCommandExecutionHelper
    {
        #region Device Commands

        public static void PressKeyCode(IExecuteMethod executeMethod, int keyCode, int metastate = -1)
        {
            var parameters = new Dictionary<string, object>()
            { ["keycode"] = keyCode };
            if (metastate > 0)
            {
                parameters.Add("metastate", metastate);
            }
            executeMethod.Execute(AppiumDriverCommand.PressKeyCode, parameters);
        }

        public static void LongPressKeyCode(IExecuteMethod executeMethod, int keyCode, int metastate = -1)
        {
            var parameters = new Dictionary<string, object>()
            { ["keycode"] = keyCode };
            if (metastate > 0)
            {
                parameters.Add("metastate", metastate);
            }
            executeMethod.Execute(AppiumDriverCommand.LongPressKeyCode, parameters);
        }

        public static void HideKeyboard(IExecuteMethod executeMethod, string strategy = null, string key = null)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (strategy != null)
            {
                parameters.Add("strategy", strategy);
            }
            if (key != null)
            {
                parameters.Add("keyName", key);
            }
            executeMethod.Execute(AppiumDriverCommand.HideKeyboard, parameters);
        }

        public static void Lock(IExecuteMethod executeMethod, int seconds) =>
            executeMethod.Execute(AppiumDriverCommand.LockDevice,
                new Dictionary<string, object>()
                { ["seconds"] = seconds });

        public static void SetClipboard(IExecuteMethod executeMethod, ClipboardContentType clipboardContentType, string base64Content)
        {
            switch (clipboardContentType)
            {
                case ClipboardContentType.Image:
                case ClipboardContentType.Url:
                    if (executeMethod.GetType().GetGenericTypeDefinition() == typeof(AndroidDriver<>))
                    { throw new NotImplementedException($"Android only supports contentType: {nameof(ClipboardContentType.PlainText)}"); }

                    executeMethod.Execute(AppiumDriverCommand.SetClipboard,
                        PrepareArguments(new[] { "content", "contentType", "label" },
                            new object[] { base64Content, clipboardContentType.ToString().ToLower(), null }));
                    break;
                default:
                    executeMethod.Execute(AppiumDriverCommand.SetClipboard,
                        PrepareArguments(new[] { "content", "contentType", "label" },
                            new object[] { base64Content, clipboardContentType.ToString().ToLower(), null }));
                    break;
            }
        }

        public static string GetClipboard(IExecuteMethod executeMethod, ClipboardContentType clipboardContentType)
        {
            switch (clipboardContentType)
            {
                case ClipboardContentType.Image:
                case ClipboardContentType.Url:
                    if (executeMethod.GetType().GetGenericTypeDefinition() == typeof(AndroidDriver<>))
                    { throw new NotImplementedException($"Android only supports contentType: {nameof(ClipboardContentType.PlainText)}"); }
                    return (string)executeMethod.Execute(AppiumDriverCommand.GetClipboard,
                        PrepareArgument("contentType", clipboardContentType.ToString().ToLower())).Value;
                case ClipboardContentType.PlainText:
                    return (string)executeMethod.Execute(AppiumDriverCommand.GetClipboard,
                        PrepareArgument("contentType", clipboardContentType.ToString().ToLower())).Value;
                default:
                    return (string)executeMethod.Execute(AppiumDriverCommand.GetClipboard,
                        PrepareArgument("contentType", clipboardContentType.ToString().ToLower())).Value;
            }
        }

        public static string SetClipboardText(IExecuteMethod executeMethod, string textContent, string label)
        {
            if (textContent == null) { throw new ArgumentException($"{nameof(textContent)} cannot be null"); }
            var encodedStringContentBytes = Encoding.UTF8.GetBytes(textContent);

            return (string)executeMethod.Execute(AppiumDriverCommand.SetClipboard,
                PrepareArguments(new[] { "content", "contentType", "label" },
                    new object[] { Convert.ToBase64String(encodedStringContentBytes), ClipboardContentType.PlainText.ToString().ToLower(), label })).Value;
        }

        public static string GetClipboardText(IExecuteMethod executeMethod)
        {
            var encodedContentBytes = Convert.FromBase64String(GetClipboard(executeMethod, ClipboardContentType.PlainText));
            return Encoding.UTF8.GetString(encodedContentBytes);
        }

        #endregion Device Commands

        /// <summary>
        /// Prepares single command argument
        /// </summary>
        /// <param name="name">The parameter name</param>
        /// <param name="value">The parameter value</param>
        /// <returns></returns>
        internal static Dictionary<string, object> PrepareArgument(string name, object value)
        {
            return new Dictionary<string, object> { { name, value } };
        }

        /// <summary>
        /// Prepares a collection of command arguments
        /// </summary>
        /// <param name="names">The array of parameter names</param>
        /// <param name="values">The array of parameter values</param>
        /// <returns></returns>
        internal static Dictionary<string, object> PrepareArguments(string[] names, object[] values)
        {
            var parameterBuilder = new Dictionary<string, object>();
            if (names.Length != values.Length) return parameterBuilder;
            for (var i = 0; i < names.Length; i++)
            {
                if (names[i] != string.Empty && values[i] != null)
                {
                    parameterBuilder.Add(names[i], values[i]);
                }
            }

            return parameterBuilder;
        }
    }
}