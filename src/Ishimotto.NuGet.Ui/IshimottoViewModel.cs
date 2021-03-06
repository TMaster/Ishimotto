﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Ishimotto.NuGet.Ui.Annotations;

namespace Ishimotto.NuGet.Ui
{
    public class IshimottoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private DateTime _fetchingDate;

        public DateTime FetchingDate
        {
            get { return _fetchingDate; }
            set
            {
                _fetchingDate = value;
                OnPropertyChanged();
                OnPropertyChanged("IsDownloadCommandEnabled");
            }
        }

        private string _packagesIds;

        public string PackagesIds
        {
            get { return _packagesIds; }
            set
            {
                _packagesIds = value;
                OnPropertyChanged();
            }
        }

        private bool _includePreRelease;

        public bool IncludePreRelease
        {
            get { return _includePreRelease; }
            set
            {
                _includePreRelease = value;
                OnPropertyChanged();
            }
        }


        public DateTime MaxFetchingDate
        {
            get { return DateTime.Now; }
        }


        private bool _isDownloadCommandEnabled;

        public DownloadInfoModel DownloadInfoModel { get; private set; }

        public bool IsDownloadCommandEnabled
        {
            get { return _isDownloadCommandEnabled && FetchingDate.CompareTo(default(DateTime)) >0; }
            set
            {
                _isDownloadCommandEnabled = value;
                OnPropertyChanged();
            }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }


        private bool _isBusy;

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value; 
                OnPropertyChanged();
            }
        }



        private bool mIsSpecifiedPackagesOnly;

        public bool IsSpecifiedPackagesOnly
        {
            get { return mIsSpecifiedPackagesOnly; }
            set
            {
                mIsSpecifiedPackagesOnly = value;
                OnPropertyChanged();
            }
        }


        
        public DownloadPakagesCommand DownloadCommand { get; set; }


        public IshimottoViewModel(DownloadInfoModel info)
        {
            DownloadCommand = new DownloadPakagesCommand(this);

            FetchingDate = DateTime.Now.AddDays(-18);

            DownloadInfoModel = info;

            IsDownloadCommandEnabled = true;

            Status = "Ready";

            IsBusy = false;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
