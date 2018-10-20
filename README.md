# NumericTextBox

## これは何？

WPFで数値入力ができるテキストボックス。

## 何がうれしい？

- 最大値・最小値の設定
- 非数値入力時に入力確定しない
- Enterで入力確定、Escapeで入力破棄

## 使い方

サンプルプロジェクトがあります。

xaml (View側)

```xml
<DoubleTextBox Text="{Binding DoubleValue, Mode=TwoWay,UpdateSourceTrigger=Explicit}"
        MaxValue="11.4" MinValue="-5.14"/>
<IntTextBox Text="{Binding IntValue, Mode=TwoWay, UpdateSourceTrigger=Explicit}"
        MaxValue="33" MinValue="-4"/>
```

c# (Binding Source側)

```csharp
public class SourceObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public double DoubleValue
        {
            get { return _doubleValue; }
            set
            {
                if(_doubleValue == value) { return; }
                _doubleValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DoubleValue)));
            }
        }
        private double _doubleValue;

        public double IntValue
        {
            get { return _intValue; }
            set
            {
                if(_intValue == value) { return; }
                _intValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IntValue)));
            }
        }
        private double _intValue;
    }
```

## その他

Bindingして使用することを前提にしています。
Bindingしない場合、ただのTextBoxです。

## 書いた人

ikorin24 ([github](https://github.com/ikorin24))