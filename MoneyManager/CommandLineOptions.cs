// =============================================================================
//  Copyright © 2017 FLIR Integrated Imaging Solutions, Inc. All Rights Reserved.
// 
//  This software is the confidential and proprietary information of FLIR
//  Integrated Imaging Solutions, Inc. ("Confidential Information"). You
//  shall not disclose such Confidential Information and shall use it only in
//  accordance with the terms of the license agreement you entered into
//  with FLIR Integrated Imaging Solutions, Inc. (FLIR).
// 
//  FLIR MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE SUITABILITY OF THE
//  SOFTWARE, EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//  PURPOSE, OR NON-INFRINGEMENT. FLIR SHALL NOT BE LIABLE FOR ANY DAMAGES
//  SUFFERED BY LICENSEE AS A RESULT OF USING, MODIFYING OR DISTRIBUTING
//  THIS SOFTWARE OR ITS DERIVATIVES.
// =============================================================================
// 

using System;
using NLog;

namespace MoneyManager
{
    public class CommandLineOptions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool PromptUser { get; private set; }
        public string TransactionLogFilePath { get; private set; }
        public double AccountBalance { get; private set; }
        public DateTime AccountBalanceDate { get; private set; }

        public bool ParseArguments(string[] args)
        {
            try
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-prompt":
                            PromptUser = true;
                            break;

                        case "-txlog":
                            TransactionLogFilePath = args[++i];
                            break;

                        case "-startBalance":
                            AccountBalance = double.Parse(args[++i]);
                            break;

                        case "-startDate":
                            AccountBalanceDate = DateTime.Parse(args[++i]);
                            break;


                        default:
                            Logger.Error($"Unrecognized parameter: {args[i]}");
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Unable to parse parameters: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}
