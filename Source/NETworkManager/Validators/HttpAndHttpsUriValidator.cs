﻿using NETworkManager.Models.Settings;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace NETworkManager.Validators
{
    public class HttpAndHttpsUriValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (Uri.TryCreate(value as string, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                return ValidationResult.ValidResult;

            return new ValidationResult(false, LocalizationManager.GetStringByKey("String_ValidationError_EnterValidWebsiteUri"));
        }
    }
}
