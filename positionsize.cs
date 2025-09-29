
#region Using declarations
using System;
using System.ComponentModel;
using System.Windows.Media;                 // Brushes
using NinjaTrader.Cbi;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.Tools;               // TextPosition, SimpleFont
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools; // HorizontalLine
using NinjaTrader.Core.FloatingPoint;
#endregion



namespace NinjaTrader.NinjaScript.Indicators{
    public class FixedRiskSizer : Indicator{
        [NinjaScriptProperty]
		public double RiskAmount { get; set; } = 200;
		
        private HorizontalLine slLine;
		private TextFixed textField;
        private double sl = 0;
		
		double tickSize;
		double tickValue;
		
		protected override void OnRender(ChartControl chartControl, ChartScale chartScale){
			if (slLine != null && sl != slLine.StartAnchor.Price){
				sl = slLine.StartAnchor.Price;
				calc();
			}
		}
        
        protected override void OnStateChange(){
            if (State == State.SetDefaults){
                Description = "Displays recommended position size based on a fixed USD risk and a draggable horizontal stop line.";
                Name = "FixRisk";
                IsOverlay = true;
                Calculate = Calculate.OnEachTick;
                IsSuspendedWhileInactive = true;
            }
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
			}
        }

        protected override void OnBarUpdate(){
			if (State != State.Realtime) return;
			calc();
		}
		
		private void calc(){
			
			//define mode for later usage
			bool shortMode = true;
			bool buyMode = false;
			double entry = GetCurrentBid();
			if (Close[0] > slLine.StartAnchor.Price){
				shortMode = false;
				buyMode = true;
				entry = GetCurrentAsk();
			}
			
			//difference
			double diff = Math.Abs(entry - sl);
			double ticksToStop = diff / tickSize;
			double riskPerContract = ticksToStop * tickValue;
			int qty = (int)Math.Floor(RiskAmount / riskPerContract);
			
			textField.DisplayText = "risk " + RiskAmount + "$, target qty is " + qty;
		}
	}
}
