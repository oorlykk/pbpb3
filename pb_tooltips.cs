using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using SnLib;

namespace pbpb
{
	public static class Hints
	{
        public const string AllowRestartPC = "Добавить в Автозапуск и разрешить перезагрузку PC при необходимости";
        public const string PubgWindowPosX = "Позиционирование основного окна PUBG относительно левого края экрана";
        public const string PubgWindowPosY = "Позиционирование основного окна PUBG относительно верхнего края экрана";
        public const string InputMethod = "Симуляция пользовательского ввода (Full - усиленный)";
        public const string ShutdowPC = "Активирует таймер выключения PC";
        public const string ShutdowPCvalue = "Время до отключения PC (в минутах)";
        public const string SaveRewards = "Делать скриншот окна игры по завернеию каждого раунда и сохранять в папку Мои изображения";
        public const string SetStyle = "Устанавливать 'безрамочный' стиль для основного окна игры";
        public const string PassiveMode = "Ввод в окно игры проиходит только при простое системы более 5 секунд";
        public const string UseFix = "Пытаться исправлять 'неточные' нажатия";
	}

    partial class Form1
    {

        void InitAppToolTips() {

            new ToolTip().SetToolTip( cbox_PubgInput, Hints.InputMethod );
            new ToolTip().SetToolTip( chb_AllowRestartPC, Hints.AllowRestartPC );
            new ToolTip().SetToolTip( ne_PosX, Hints.PubgWindowPosX );
            new ToolTip().SetToolTip( ne_PosY, Hints.PubgWindowPosY );
            new ToolTip().SetToolTip( chb_shutdownpcafter, Hints.ShutdowPC );
            new ToolTip().SetToolTip( ne_shutdownpcafter, Hints.ShutdowPCvalue );
            new ToolTip().SetToolTip( lab_shutdownpcafter, Hints.ShutdowPCvalue );
            new ToolTip().SetToolTip( chb_SaveReward, Hints.SaveRewards );
            new ToolTip().SetToolTip( chb_PassiveMode, Hints.PassiveMode );
            new ToolTip().SetToolTip( chb_SetStyle, Hints.SetStyle );
            new ToolTip().SetToolTip( chb_FixMissStates, Hints.UseFix );
        }
    }
}
