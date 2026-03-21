//using System;
//using cAlgo.API;
//using cAlgo.API.Collections;
//using cAlgo.API.Indicators;
//using cAlgo.API.Internals;

//namespace cAlgo.Robots
//{
//    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, AddIndicators = true)]
//    public class OneDayCrossRsiVol : Robot
//    {
//        private double _volumeInUnits;

//        private MovingAverage _fastMa;

//        private MovingAverage _slowMa;

//        [Parameter("Break-even Trigger (pips)", Group = "Protection", DefaultValue = 1000, MinValue = 1)]
//        public int BreakEvenTriggerInPips { get; set; }

//        [Parameter("Source", Group = "Fast MA")]
//        public DataSeries FastMaSource { get; set; }

//        [Parameter("Period", DefaultValue = 10, Group = "Fast MA")]
//        public int FastMaPeriod { get; set; }

//        [Parameter("Type", DefaultValue = MovingAverageType.Exponential, Group = "Fast MA")]
//        public MovingAverageType FastMaType { get; set; }

//        [Parameter("Source", Group = "Slow MA")]
//        public DataSeries SlowMaSource { get; set; }

//        [Parameter("Period", DefaultValue = 20, Group = "Slow MA")]
//        public int SlowMaPeriod { get; set; }

//        [Parameter("Type", DefaultValue = MovingAverageType.Exponential, Group = "Slow MA")]
//        public MovingAverageType SlowMaType { get; set; }

//        [Parameter("Volume (Lots)", DefaultValue = 1, Group = "Trade")]
//        public double VolumeInLots { get; set; }

//        [Parameter("Stop Loss (Pips)", DefaultValue = 500, MaxValue = 200000, MinValue = 1, Step = 1)]
//        public double StopLossInPips { get; set; }

//        [Parameter("Take Profit (Pips)", DefaultValue = 100000, MaxValue = 200000, MinValue = 1, Step = 1)]
//        public double TakeProfitInPips { get; set; }

//        [Parameter("Label", DefaultValue = "cross-rsi-vol3", Group = "Trade")]
//        public string Label { get; set; }

//        [Parameter("RSI Period", DefaultValue = 14)]
//        public int RSIPeriod { get; set; }

//        [Parameter("RSI Buy Threshold", DefaultValue = 50)]
//        public double RSIThresholdBuy { get; set; }

//        [Parameter("RSI Sell Threshold", DefaultValue = 40)]
//        public double RSIThresholdSell { get; set; }

//        [Parameter("Min Volume", DefaultValue = 100000)]
//        public int MinVolume { get; set; }

//        [Parameter("GoLongAtStart", DefaultValue = false, Group = "Trade At Startup")]
//        public bool GoLongAtStart { get; set; }

//        [Parameter("GoShortAtStart", DefaultValue = false, Group = "Trade At Startup")]
//        public bool GoShortAtStart { get; set; }

//        [Parameter("MaxPositions", DefaultValue = 2, Group = "Trade At Startup")]
//        public int MaxPositions { get; set; }

//        private RelativeStrengthIndex _rsi;


//        public Position[] BotPositions
//        {
//            get
//            {
//                return Positions.FindAll(Label);
//            }
//        }

//        protected override void OnStart()
//        {
//            _volumeInUnits = Symbol.QuantityToVolumeInUnits(VolumeInLots);

//            _fastMa = Indicators.MovingAverage(FastMaSource, FastMaPeriod, FastMaType);
//            _slowMa = Indicators.MovingAverage(SlowMaSource, SlowMaPeriod, SlowMaType);

//            _fastMa.Result.LineOutput.Color = Color.Red;
//            _slowMa.Result.LineOutput.Color = Color.Red;

//            _rsi = Indicators.RelativeStrengthIndex(Bars.ClosePrices, RSIPeriod);

//            if (GoLongAtStart)
//                ExecuteMarketOrder(TradeType.Buy, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
//            if (GoShortAtStart)
//                ExecuteMarketOrder(TradeType.Sell, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);

//        }

//        protected override void OnTick()
//        {
//            foreach (var position in BotPositions)
//            {
//                double profitInPips = position.GrossProfit / Symbol.PipValue;

//                if (profitInPips >= BreakEvenTriggerInPips)
//                {
//                    double breakEvenPrice = position.EntryPrice;

//                    if (position.TradeType == TradeType.Buy)
//                    {
//                        if (position.StopLoss == null || position.StopLoss < breakEvenPrice)
//                            ModifyPosition(position, breakEvenPrice, position.TakeProfit, ProtectionType.Relative);
//                    }
//                    else if (position.TradeType == TradeType.Sell)
//                    {
//                        if (position.StopLoss == null || position.StopLoss > breakEvenPrice)
//                            ModifyPosition(position, breakEvenPrice, position.TakeProfit, ProtectionType.Relative);
//                    }
//                }
//            }
//        }


//        protected override void OnBarClosed()
//        {
//            int index = Bars.Count - 1;

//            bool volumeOk = Bars.TickVolumes[index] >= MinVolume;

//            double rsiValue = _rsi.Result[index];

//            if (_fastMa.Result.HasCrossedAbove(_slowMa.Result, 0)
//                && rsiValue > RSIThresholdBuy && volumeOk)
//            {
//                ClosePositions(TradeType.Sell);

//                if (BotPositions.Length >= MaxPositions)
//                {
//                    // If we have reached the maximum number of positions, do not open a new one
//                    return;
//                }

//                ExecuteMarketOrder(TradeType.Buy, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
//            }
//            else if (_fastMa.Result.HasCrossedBelow(_slowMa.Result, 0)
//                && rsiValue < RSIThresholdSell && volumeOk)
//            {
//                ClosePositions(TradeType.Buy);

//                if (BotPositions.Length >= MaxPositions)
//                {
//                    // If we have reached the maximum number of positions, do not open a new one
//                    return;
//                }

//                ExecuteMarketOrder(TradeType.Sell, SymbolName, _volumeInUnits, Label, StopLossInPips, TakeProfitInPips);
//            }
//        }

//        private void ClosePositions(TradeType tradeType)
//        {
//            foreach (var position in BotPositions)
//            {
//                if (position.TradeType != tradeType) continue;

//                ClosePosition(position);
//            }
//        }
//    }
//}