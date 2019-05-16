﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

using System;
using System.Globalization;
using System.IO;

namespace QuantConnect.Data.Custom.PsychSignal
{
    public class PsychSignalSentimentData : BaseData
    {
        /// <summary>
        /// Bullish intensity as reported by psychsignal
        /// </summary>
        public decimal BullIntensity;

        /// <summary>
        /// Bearish intensity as reported by psychsignal
        /// </summary>
        public decimal BearIntensity;

        /// <summary>
        /// Bullish intensity minus bearish intensity
        /// </summary>
        public decimal BullMinusBear;

        /// <summary>
        /// Total bullish scored messages
        /// </summary>
        public int BullScoredMessages;

        /// <summary>
        /// Total bearish scored messages
        /// </summary>
        public int BearScoredMessages;

        /// <summary>
        /// Bull/Bear message ratio.
        /// </summary>
        /// <remarks>If bearish messages equals zero, then the resulting value equals zero</remarks>
        public decimal BullBearMessageRatio;

        /// <summary>
        /// Total messages scanned.
        /// </summary>
        /// <remarks>
        /// Sometimes, there will be no bull/bear rated messages, but nonetheless had messages scanned.
        /// This field describes the total fields that were scanned in a minute
        /// </remarks>
        public int TotalScoredMessages;

        /// <summary>
        /// Retrieve Psychsignal data from disk and return it to user's custom data subscription
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <param name="date">Date of this source file</param>
        /// <param name="isLiveMode">true if we're in livemode, false for backtesting mode</param>
        /// <returns></returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime date, bool isLiveMode)
        {
            return new SubscriptionDataSource(
                Path.Combine(
                    Globals.DataFolder,
                    "alternative",
                    "psychsignal",
                    $"{config.Symbol.Value.ToLower()}.zip#{date:yyyyMMdd}.csv"
                ),
                SubscriptionTransportMedium.LocalFile,
                FileFormat.Csv
            );
        }

        /// <summary>
        /// Reads a single entry from psychsignal's data source.
        /// </summary>
        /// <param name="config">Subscription data config setup object</param>
        /// <param name="line">Line of the source document</param>
        /// <param name="date">Date of the requested data</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>
        ///     Instance of the T:BaseData object containing psychsignal specific data
        /// </returns> 
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime date, bool isLiveMode)
        {
            try
            {
                var csv = line.Split(',');

                var timestamp = new DateTime(date.Year, date.Month, date.Day).AddMilliseconds(
                    Convert.ToDouble(csv[0])
                );
                var bullIntensity = Convert.ToDecimal(csv[1], CultureInfo.InvariantCulture);
                var bearIntensity = Convert.ToDecimal(csv[2], CultureInfo.InvariantCulture);
                var bullMinusBear = Convert.ToDecimal(csv[3], CultureInfo.InvariantCulture);
                var bullScoredMessages = Convert.ToInt32(csv[4]);
                var bearScoredMessages = Convert.ToInt32(csv[5]);
                var bullBearMessageRatio = Convert.ToDecimal(csv[6], CultureInfo.InvariantCulture);
                var totalScannedMessages = Convert.ToInt32(csv[7]);
                
                return new PsychSignalSentimentData
                {
                    Time = timestamp,
                    Symbol = config.Symbol,
                    BullIntensity = bullIntensity,
                    BearIntensity = bearIntensity,
                    BullMinusBear = bullMinusBear,
                    BullScoredMessages = bullScoredMessages,
                    BearScoredMessages = bearScoredMessages,
                    BullBearMessageRatio = bullBearMessageRatio,
                    TotalScoredMessages = totalScannedMessages
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
