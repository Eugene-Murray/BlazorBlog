//using System;
//using cAlgo.API;
//using cAlgo.API.Indicators;

//namespace cAlgo.Robots
//{
//    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
//    public class FiveMinuteCrossBot : Robot
//    {
//        [Parameter("Fast MA Period", DefaultValue = 20)]
//        public int FastPeriod { get; set; }

//        [Parameter("Slow MA Period", DefaultValue = 50)]
//        public int SlowPeriod { get; set; }

//        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
//        public MovingAverageType MAType { get; set; }

//        [Parameter("Volume", DefaultValue = 0.5)]
//        public double Volume { get; set; }

//        [Parameter("Stop Loss (pips)", DefaultValue = 20)]
//        public int StopLossPips { get; set; }

//        [Parameter("Take Profit (pips)", DefaultValue = 40)]
//        public int TakeProfitPips { get; set; }

//        [Parameter("Go Long AtStart", DefaultValue = false)]
//        public bool GoLongAtStart { get; set; }

//        [Parameter("Go Short AtStart", DefaultValue = false)]
//        public bool GoShortAtStart { get; set; }

//        [Parameter("Max Loss Per Day", DefaultValue = 500.0, MinValue = 0)]
//        public double MaxLossPerDay { get; set; }

//        [Parameter("StopLoss Trigger (pips)", DefaultValue = 200, MinValue = 1)]
//        public int StopLossTriggerPips { get; set; }

//        [Parameter("Tightened StopLoss (pips)", DefaultValue = 100, MinValue = 1)]
//        public int TightenedStopLossPips { get; set; }

//        [Parameter("Max Open Positions", DefaultValue = 1, MinValue = 1)]
//        public int MaxOpenPositions { get; set; }

//        private MovingAverage _fastMa;
//        private MovingAverage _slowMa;

//        private double _startOfDayEquity;
//        private DateTime _currentDay;

//        protected override void OnStart()
//        {
//            _fastMa = Indicators.MovingAverage(Bars.ClosePrices, FastPeriod, MAType);
//            _slowMa = Indicators.MovingAverage(Bars.ClosePrices, SlowPeriod, MAType);
//            _fastMa.Result.LineOutput.Color = Color.Red;
//            _slowMa.Result.LineOutput.Color = Color.Red;

//            _currentDay = Server.Time.Date;
//            _startOfDayEquity = Account.Equity;

//            if (GoLongAtStart)
//                ExecuteMarketOrder(TradeType.Buy, SymbolName, Volume, "CrossoverBot", StopLossPips, TakeProfitPips);
//            if (GoShortAtStart)
//                ExecuteMarketOrder(TradeType.Sell, SymbolName, Volume, "CrossoverBot", StopLossPips, TakeProfitPips);
//        }

//        protected override void OnTick()
//        {
//            foreach (var position in Positions.FindAll("CrossoverBot", SymbolName))
//            {
//                if (position.Pips <= -StopLossTriggerPips)
//                    TightenStopLoss(position);
//            }
//        }

//        protected override void OnBar()
//        {
//            if (IsDailyLossLimitReached())
//            {
//                ClosePositions(TradeType.Sell);
//                ClosePositions(TradeType.Buy);
//                Print("Max daily loss of {0} reached. No new trades will be opened today.", MaxLossPerDay);
//                return;
//            }

//            bool bullishCross = _fastMa.Result.Last(1) < _slowMa.Result.Last(1) && _fastMa.Result.Last(0) > _slowMa.Result.Last(0);
//            bool bearishCross = _fastMa.Result.Last(1) > _slowMa.Result.Last(1) && _fastMa.Result.Last(0) < _slowMa.Result.Last(0);

//            int openCount = Positions.FindAll("CrossoverBot", SymbolName).Length;

//            if (bullishCross)
//            {
//                ClosePositions(TradeType.Sell);
//                if (openCount < MaxOpenPositions)
//                    ExecuteMarketOrder(TradeType.Buy, SymbolName, Volume, "CrossoverBot", StopLossPips, TakeProfitPips);
//            }
//            else if (bearishCross)
//            {
//                ClosePositions(TradeType.Buy);
//                if (openCount < MaxOpenPositions)
//                    ExecuteMarketOrder(TradeType.Sell, SymbolName, Volume, "CrossoverBot", StopLossPips, TakeProfitPips);
//            }
//        }

//        private bool IsDailyLossLimitReached()
//        {
//            if (MaxLossPerDay <= 0)
//                return false;

//            var today = Server.Time.Date;
//            if (today != _currentDay)
//            {
//                _currentDay = today;
//                _startOfDayEquity = Account.Equity;
//            }

//            double dailyLoss = _startOfDayEquity - Account.Equity;
//            return dailyLoss >= MaxLossPerDay;
//        }

//        private void ClosePositions(TradeType type)
//        {
//            foreach (var position in Positions.FindAll("CrossoverBot", SymbolName, type))
//                ClosePosition(position);
//        }

//        private void TightenStopLoss(Position position)
//        {
//            double pipSize = Symbol.PipSize;

//            if (position.TradeType == TradeType.Buy)
//            {
//                double newSl = Symbol.Bid - TightenedStopLossPips * pipSize;
//                if (position.StopLoss == null || newSl > position.StopLoss.Value)
//                    ModifyPosition(position, newSl, position.TakeProfit, ProtectionType.Absolute);
//            }
//            else
//            {
//                double newSl = Symbol.Ask + TightenedStopLossPips * pipSize;
//                if (position.StopLoss == null || newSl < position.StopLoss.Value)
//                    ModifyPosition(position, newSl, position.TakeProfit, ProtectionType.Absolute);
//            }
//        }
//    }
//}