#region Using declarations
using System;
using System.Windows.Media;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.Core.FloatingPoint;
using System.Windows;
#endregion

namespace NinjaTrader.NinjaScript.Indicators{
    public class FixedRiskSizer : Indicator{
        [NinjaScriptProperty]
		public double RiskAmount { get; set; } = 200;
		
        private HorizontalLine slLine = null;
		private TextFixed textField = null;
        private double sl = 0;
		private QuantityUpDown qtyField = null;
		
		double lastClose = 0;
		double tickSize = 0;
		double tickValue = 0;
		
		//UI update, for example line drag n drop
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale){
			calc();
		}
        
        protected override void OnStateChange(){
			//default load
            if (State == State.SetDefaults){
                Description = "Displays recommended position size & update qty on chart trader based on a fixed USD risk and a draggable horizontal stop line.";
                Name = "FixRisk";
                IsOverlay = true;
                Calculate = Calculate.OnEachTick;
            }
			
			//on realtime
			if (State == State.Realtime){
				//SL line
				sl = Close[0];
				slLine = Draw.HorizontalLine(this, "RiskCalculatorSL", sl, Brushes.Red);
				slLine.IsLocked = false;
				
				//Text
				textField = Draw.TextFixed(this, "RiskCalculatorText", "", TextPosition.TopLeft, Brushes.Black, new SimpleFont("Segoe UI", 12), Brushes.Transparent, Brushes.Transparent, 0);
				
				//load instrument data
				tickSize = Instrument.MasterInstrument.TickSize;
				double pointValue = Instrument.MasterInstrument.PointValue;
				tickValue = pointValue * tickSize;
				
				//get the quantity control field
				ChartControl.Dispatcher.InvokeAsync((Action)(() => {
					qtyField = Window.GetWindow(ChartControl.Parent).FindFirst("ChartTraderControlQuantitySelector") as QuantityUpDown;
				}));
				
			}
        }
		
        protected override void OnBarUpdate(){
			if (State != State.Realtime) return;
			calc();
		}
		
		
		private void calc(){
			
			//if nothing changed or something is not ready, not need to recalc again
			if (
				slLine == null || qtyField == null
				|| (sl == slLine.StartAnchor.Price && lastClose == Close[0])
			) return;
			
			//update variables
			lastClose = Close[0];
			sl = slLine.StartAnchor.Price;
			
			//define mode for later usage
			double entry = GetCurrentBid();
			if (Close[0] > slLine.StartAnchor.Price) entry = GetCurrentAsk();
			
			//difference & calc
			double diff = Math.Abs(entry - sl);
			double ticksToStop = diff / tickSize;
			double riskPerContract = ticksToStop * tickValue;
			int qty = (int)Math.Floor(RiskAmount / riskPerContract);
			
			//display on chart and update qty on chart trader
			textField.DisplayText = "to risk " + RiskAmount + "$, target qty is " + qty;
			ChartControl.Dispatcher.InvokeAsync((Action)(() => {
				qtyField.Value = qty;
			}));
			
		}
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private FixedRiskSizer[] cacheFixedRiskSizer;
		public FixedRiskSizer FixedRiskSizer(double riskAmount)
		{
			return FixedRiskSizer(Input, riskAmount);
		}

		public FixedRiskSizer FixedRiskSizer(ISeries<double> input, double riskAmount)
		{
			if (cacheFixedRiskSizer != null)
				for (int idx = 0; idx < cacheFixedRiskSizer.Length; idx++)
					if (cacheFixedRiskSizer[idx] != null && cacheFixedRiskSizer[idx].RiskAmount == riskAmount && cacheFixedRiskSizer[idx].EqualsInput(input))
						return cacheFixedRiskSizer[idx];
			return CacheIndicator<FixedRiskSizer>(new FixedRiskSizer(){ RiskAmount = riskAmount }, input, ref cacheFixedRiskSizer);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.FixedRiskSizer FixedRiskSizer(double riskAmount)
		{
			return indicator.FixedRiskSizer(Input, riskAmount);
		}

		public Indicators.FixedRiskSizer FixedRiskSizer(ISeries<double> input , double riskAmount)
		{
			return indicator.FixedRiskSizer(input, riskAmount);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.FixedRiskSizer FixedRiskSizer(double riskAmount)
		{
			return indicator.FixedRiskSizer(Input, riskAmount);
		}

		public Indicators.FixedRiskSizer FixedRiskSizer(ISeries<double> input , double riskAmount)
		{
			return indicator.FixedRiskSizer(input, riskAmount);
		}
	}
}

#endregion
