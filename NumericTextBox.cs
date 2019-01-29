using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace Ikorin
{
    #region class NumericTextBoxBase<T>
    /// <summary>数値入力テキストボックスの基底クラス</summary>
    /// <typeparam name="T">数値の型</typeparam>
    public abstract class NumericTextBoxBase<T> : TextBox where T : struct, IComparable<T>
    {
        private readonly static CustomValidationRule _customRule = new CustomValidationRule();
        private bool _addedRule;

        #region MaxValue
        /// <summary>入力可能な数値の最大値</summary>
        public T MaxValue
        {
            get { return (T)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// <see cref="MaxValue"/> 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(T), typeof(NumericTextBoxBase<T>), new PropertyMetadata());
        #endregion MaxValue

        #region MinValue
        /// <summary>入力可能な数値の最小値</summary>
        public T MinValue
        {
            get { return (T)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// <see cref="MinValue"/> 依存関係プロパティ
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(T), typeof(NumericTextBoxBase<T>), new PropertyMetadata());
        #endregion MinValue

        /// <summary>コンストラクタ</summary>
        public NumericTextBoxBase()
        {
            SetDefaultMinMax();
        }

        #region Method
        /// <summary>派生クラスで、文字列から値への変換を行います</summary>
        /// <param name="text">文字列</param>
        /// <param name="value">変換された値</param>
        /// <returns>変換可能か</returns>
        protected abstract bool ValueTryParse(string text, out T value);

        /// <summary>派生クラスで、最大値と最小値の初期値を設定します</summary>
        protected abstract void SetDefaultMinMax();

        /// <summary>キー入力時処理</summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Enter押下時に入力文字列が数値ならMax値, Min値で入力値を修正して確定(バインドソースを更新)
            var be = GetBindingExpression(TextProperty);
            if(be == null) { return; }  // バインドされていなければ処理をしない
            if(!_addedRule) {
                be.ParentBinding.ValidationRules.Add(_customRule);
                _addedRule = true;
            }
            if(e.Key == Key.Enter) {
                if(ValueTryParse(Text, out var value)) {
                    if(value.CompareTo(MaxValue) > 0) {
                        value = MaxValue;
                        Text = value.ToString();
                    }
                    if(value.CompareTo(MinValue) < 0) {
                        value = MinValue;
                        Text = value.ToString();
                    }
                    be.UpdateSource();
                    Select(Text.Length, 0);     // カーソルを最後尾に
                }
                else {
                    SelectAll();
                    be.UpdateSource();  // 内部で型変換例外が発生してバインドソースの更新失敗、テキストボックスの枠がError色に変化
                }
            }
            // Escape押下時はバインドソースから値をレストア
            else if(e.Key == Key.Escape) {
                be.UpdateTarget();
                Select(Text.Length, 0);     // カーソルを最後尾に
            }
            base.OnKeyDown(e);
        }

        /// <summary>フォーカスロスト時処理</summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            var be = GetBindingExpression(TextProperty);
            if(be == null) { return; }      // バインドされていなければ処理をしない
            // 入力文字列が数値ならMax値, Min値で入力値を修正して確定(バインドソースを更新)
            if(ValueTryParse(Text, out var value)) {
                if(value.CompareTo(MaxValue) > 0) {
                    value = MaxValue;
                    Text = value.ToString();
                }
                if(value.CompareTo(MinValue) < 0) {
                    value = MinValue;
                    Text = value.ToString();
                }
                be.UpdateSource();
            }
            // 非数値ならバインドソースから値をレストア
            else {
                be.UpdateTarget();
            }
            Select(Text.Length, 0);     // カーソルを最後尾に
            base.OnLostFocus(e);
        }
        #endregion Method

        #region class CustomValidationRule
        class CustomValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                // 4e5, -3E9 等の有効数字形を許可しない
                if(value.ToString().ToLower().Contains("e")) {
                    return new ValidationResult(false, null);
                }
                return ValidationResult.ValidResult;
            }
        }
        #endregion
    }
    #endregion NumericTextBoxBase<T>

    #region class DoubleTextBox
    /// <summary>
    /// <see cref="double"/>型の値の入力を受け付けるテキストボックス
    /// </summary>
    public class DoubleTextBox : NumericTextBoxBase<double>
    {
        /// <summary>最大値と最小値の初期値を設定します</summary>
        protected override void SetDefaultMinMax()
        {
            MaxValue = double.PositiveInfinity;
            MinValue = double.NegativeInfinity;
        }

        /// <summary>文字列から値への変換を行います</summary>
        /// <param name="text">文字列</param>
        /// <param name="value">変換された値</param>
        /// <returns>変換可能か</returns>
        protected override bool ValueTryParse(string text, out double value)
        {
            if(text.ToLower().Contains("e")) {  // 有効数字形を不可とする
                value = default(double);
                return false;
            }
            return double.TryParse(text, out value);
        }
    }
    #endregion class DoubleTextBox

    #region class IntTextBox
    /// <summary>
    /// <see cref="int"/>型の値の入力を受け付けるテキストボックス
    /// </summary>
    public class IntTextBox : NumericTextBoxBase<int>
    {
        /// <summary>最大値と最小値の初期値を設定します</summary>
        protected override void SetDefaultMinMax()
        {
            MaxValue = int.MaxValue;
            MinValue = int.MinValue;
        }

        /// <summary>文字列から値への変換を行います</summary>
        /// <param name="text">文字列</param>
        /// <param name="value">変換された値</param>
        /// <returns>変換可能か</returns>
        protected override bool ValueTryParse(string text, out int value)
        {
            return int.TryParse(text, out value);
        }
    }
    #endregion class IntTextBox
}