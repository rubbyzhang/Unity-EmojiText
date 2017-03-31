using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;

namespace UIWidgets {
	/// <summary>
	/// On change event.
	/// </summary>
	[Serializable]
	public class OnChangeEventFloat : UnityEvent<float> {
	}

	/// <summary>
	/// Submit event.
	/// </summary>
	[Serializable]
	public class SubmitEventFloat : UnityEvent<float> {
	}
	
	[AddComponentMenu("UI/SpinnerFloat", 270)]
	/// <summary>
	/// Spinner.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Spinner.png
	/// </summary>
	public class SpinnerFloat : SpinnerBase<float> {
		[SerializeField]
		string format = "0.00";

		/// <summary>
		/// Gets or sets the format.
		/// </summary>
		/// <value>The format.</value>
		public string Format {
			get {
				return format;
			}
			set {
				format = value;
				text = _value.ToString(format);
			}
		}

		/// <summary>
		/// onValueChange event.
		/// </summary>
		public OnChangeEventFloat onValueChangeFloat = new OnChangeEventFloat();
		
		/// <summary>
		/// onEndEdit event.
		/// </summary>
		public SubmitEventFloat onEndEditFloat = new SubmitEventFloat();

		System.Globalization.NumberStyles NumberStyle = System.Globalization.NumberStyles.AllowDecimalPoint
			| System.Globalization.NumberStyles.AllowThousands
				| System.Globalization.NumberStyles.AllowLeadingSign;
		
		System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.SpinnerFloat"/> class.
		/// </summary>
		public SpinnerFloat()
		{
			_max = 100f;
			_step = 1f;
		}

		/// <summary>
		/// Increase value on step.
		/// </summary>
		public override void Plus()
		{
			Value += Step;
		}
		
		/// <summary>
		/// Decrease value on step.
		/// </summary>
		public override void Minus()
		{
			Value -= Step;
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="newValue">New value.</param>
		protected override void SetValue(float newValue)
		{
			if (_value==InBounds(newValue))
			{
				return ;
			}
			_value = InBounds(newValue);

			text = _value.ToString(format);
			onValueChangeFloat.Invoke(_value);
		}

		/// <summary>
		/// Called when value changed.
		/// </summary>
		/// <param name="value">Value.</param>
		protected override void ValueChange(string value)
		{
			if (SpinnerValidation.OnEndInput==Validation)
			{
				return ;
			}
			if (value.Length==0)
			{
				value = "0";
			}
			float new_value;
			if (float.TryParse(value, NumberStyle, Culture, out new_value))
			{
				SetValue(new_value);
			}
		}

		/// <summary>
		/// Called when end edit.
		/// </summary>
		/// <param name="value">Value.</param>
		protected override void ValueEndEdit(string value)
		{
			if (value.Length==0)
			{
				value = "0";
			}
			float new_value;
			if (float.TryParse(value, NumberStyle, Culture, out new_value))
			{
				SetValue(new_value);
			}
			onEndEditFloat.Invoke(_value);
		}

		/// <summary>
		/// Validate when key down for Validation=OnEndInput.
		/// </summary>
		/// <returns>The char.</returns>
		/// <param name="validateText">Validate text.</param>
		/// <param name="charIndex">Char index.</param>
		/// <param name="addedChar">Added char.</param>
		protected override char ValidateShort(string validateText, int charIndex, char addedChar)
		{
			if (charIndex != 0 || validateText.Length <= 0 || validateText [0] != '-')
			{
				if (addedChar >= '0' && addedChar <= '9')
				{
					return addedChar;
				}
				if (addedChar == '-' && charIndex == 0 && Min < 0)
				{
					return addedChar;
				}
				if (addedChar == '.' && characterValidation == InputField.CharacterValidation.Decimal && !validateText.Contains ("."))
				{
					return addedChar;
				}
			}
			return '\0';
		}

		/// <summary>
		/// Validates when key down for Validation=OnKeyDown.
		/// </summary>
		/// <returns>The char.</returns>
		/// <param name="validateText">Validate text.</param>
		/// <param name="charIndex">Char index.</param>
		/// <param name="addedChar">Added char.</param>
		protected override char ValidateFull(string validateText, int charIndex, char addedChar)
		{
			var number = validateText.Insert(charIndex, addedChar.ToString());
			
			if ((Min > 0) && (charIndex==0) && (addedChar=='-'))
			{
				return (char)0;
			}
			
			var min_parse_length = ((number.Length > 0) && (number[0]=='-')) ? 2 : 1;
			if (number.Length >= min_parse_length)
			{
				float new_value;
				if (!float.TryParse(number, NumberStyle, Culture, out new_value))
				{
					return (char)0;
				}

				if (new_value!=InBounds(new_value))
				{
					return (char)0;
				}
				
				_value = new_value;
			}
			
			return addedChar;
		}

		/// <summary>
		/// Clamps a value between a minimum and maximum value.
		/// </summary>
		/// <returns>The bounds.</returns>
		/// <param name="value">Value.</param>
		protected override float InBounds(float value)
		{
			return Mathf.Clamp(value, _min, _max);
		}

		#if UNITY_EDITOR
		[UnityEditor.MenuItem("GameObject/UI/SpinnerFloat", false, 1170)]
		static void CreateObject()
		{
			Utilites.CreateWidgetFromAsset("SpinnerFloat");
		}
		#endif
	}
}