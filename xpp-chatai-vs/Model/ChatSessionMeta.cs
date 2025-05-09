﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace xpp_chatai_vs.Model
{
    /// <summary>
    /// Chat session meta class
    /// Willie Yao - 04/14/2025
    /// </summary>
    public class ChatSessionMeta : INotifyPropertyChanged
    {
        public Guid SessionId { get; } = Guid.NewGuid();

        private string _sessionName;
        public string SessionName 
        { 
            get => _sessionName;
            set
            {
                _sessionName = value;
                OnPropertyChanged();
            }
        }        

        public DateTime LastActiveTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }   
}
