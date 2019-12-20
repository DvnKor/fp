﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CommandLine;
using TagsCloudContainer.Parsing;
using TagsCloudContainer.ResultInfrastructure;
using TagsCloudContainer.Visualization;

namespace TagsCloudContainer.Settings_Providing
{
    public class SettingsProvider : ISettingsProvider
    {
        private Result<Settings> settings;

        public SettingsProvider(IFileParser parser)
        {
            Parser.Default.ParseArguments<Options>(new[] {""}).WithParsed(opts => settings = GetSettings(opts, parser));
        }

        public SettingsProvider(Options options, IFileParser parser)
        {
            settings = GetSettings(options, parser);
        }

        public Result<Settings> GetSettings()
        {
            return settings;
        }

        public ColoringOptions GetColoringOptions()
        {
            return settings.Value.ColoringOptions;
        }

        private static Result<Settings> GetSettings(Options options, IFileParser parser)
        {
            try
            {
                var coloringOptions = GetColoringOptions(options);
                var excludedWords = GetWordsHashSet(options.ExcludedWordsPath, parser);
                var excludedPartsOfSpeech = GetWordsHashSet(options.ExcludedPartsOfSpeechPath, parser);
                var resolution = GetSizeFromString(options.ResolutionString);
                return ResultExtensions.Ok(new Settings(options.InputPath, options.OutputPath, coloringOptions,
                    excludedWords,
                    excludedPartsOfSpeech, resolution, options.FontName));
            }
            catch (Exception e)
            {
                return ResultExtensions.Fail<Settings>(e.Message);
            }
        }

        private static ColoringOptions GetColoringOptions(Options options)
        {
            var rectangleFillBrush = GetBrushFromColorName(options.RectangleColorWord);
            var backgroundFillBrush = GetBrushFromColorName(options.BackgroundColorWord);
            var rectangleBorderPen = GetPenFromColorName(options.RectangleBorderColorWord);
            var textBrush = GetBrushFromColorName(options.FontColorWord);
            return new ColoringOptions(rectangleFillBrush, backgroundFillBrush, rectangleBorderPen, textBrush);
        }

        private static Pen GetPenFromColorName(string colorName)
        {
            return new Pen(Color.FromName(colorName));
        }

        private static Brush GetBrushFromColorName(string colorName)
        {
            return new SolidBrush(Color.FromName(colorName));
        }

        private static HashSet<string> GetWordsHashSet(string path, IFileParser parser)
        {
            return parser.ParseFile(path).Value.ToHashSet();
        }

        private static Size GetSizeFromString(string sizeString)
        {
            var sizes = sizeString.Split('x');
            return new Size(int.Parse(sizes[0]), int.Parse(sizes[1]));
        }
    }
}