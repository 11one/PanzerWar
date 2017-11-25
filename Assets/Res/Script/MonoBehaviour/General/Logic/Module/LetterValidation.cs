using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialUI;
using System.Text.RegularExpressions;

public class LetterValidation : MonoBehaviour, ITextValidator
{
	private MaterialInputField m_MaterialInputField;

	public void Init(MaterialInputField materialInputField)
	{
		m_MaterialInputField = materialInputField;
	}

	public bool IsTextValid()
	{
		if (ValidUtil.IsUsername(m_MaterialInputField.inputField.text))
		{
			m_MaterialInputField.validationText.text = "错误!只能输入字母与数字";
			return false;
		}
		else
		{
			return true;
		}	
	}
}