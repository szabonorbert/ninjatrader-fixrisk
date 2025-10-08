# NinjaTrader FixRisk

A simple **NinjaTrader 8 indicator** for quick and precise **position sizing** based on your desired risk per trade and stop distance.   Built for professional discretionary futures and forex traders who prefer clear, rule-based risk management.

## ⚙️ Features

- Calculates optimal position size based on:
  - Fixed risk amount (in account currency)
  - Stop-loss distance (set by chart line)
  - Instrument tick value
- Automatically fills the **Chart Trader → QTY** field
- Displays position size and risk info directly on the chart
- Lightweight, zero-dependency, open-source

## 📦 Installation

1. Download `FixRisk.cs`
2. In NinjaTrader:
   - Go to **New → NinjaScript Editor**
   - Right-click the **Indicators** folder → **New Indicator**
3. Press **Generate**
4. Remove everything
5. Paste the content of ```FixRisk.cs```
6. Press compile button ![](compile.png) on the top of the NinjaScript Editor window 
4. After the compile finished, add *FixRisk* to your chart

## 🧠 Usage

1. Set your desired risk per trade (in account currency)
2. Define your stop-loss using the **red line** on the chart
3. The indicator calculates the optimal position size and updates:
   - On-screen text (position info)
   - Chart Trader’s **QTY** input automatically

![](fixrisk.gif)

## 🧾 License

This project is licensed under the [MIT License](./LICENSE).

## 👤 Author

**Norbert Szabo**  
CTO, developer and part-time trader