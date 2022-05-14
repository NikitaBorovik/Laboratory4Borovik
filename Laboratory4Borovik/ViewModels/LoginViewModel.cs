﻿using System;
using Laboratory4Borovik.Tools;
using Laboratory4Borovik.Models;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Laboratory4Borovik.Navigation;
using System.Threading.Tasks;
using Laboratory4Borovik.Sending;
using System.Text.RegularExpressions;
using Laboratory4Borovik.Exceptions;
using Laboratory4Borovik.Repository;

namespace Laboratory4Borovik.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged, INavigatable
    {
        private RelayCommand<object>? gotoInfoCommand;
        private RelayCommand<object>? cancelCommand;
        private Person ourPerson;
        private Action gotoInfo;
        private DateTime birthDate = DateTime.Today;
        public event PropertyChangedEventHandler? PropertyChanged;
        public NavigationTypes ViewType => NavigationTypes.Login;
        public DateTime BirthDate { get => birthDate; set => birthDate = value; }
        private static PersonFileRepository PersonFileRepository = new PersonFileRepository();
        
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public RelayCommand<object> ProceedCommand
        {
            get
            {
                return gotoInfoCommand ??= new RelayCommand<object>(_ => Proceed(), CanExecute);
            }
        }

        
        public RelayCommand<object> CancelCommand
        {
            get
            {
                return cancelCommand ??= new RelayCommand<object>(o => Cancel());
            }
        }

        public LoginViewModel(Action gotoInfo)
        {
            this.gotoInfo = gotoInfo;
        }
        private void Cancel()
        {
            gotoInfo.Invoke();
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task Proceed()
        {
            bool isAdult;
            string sunSign;
            string chineseSign;
            bool isBirthday;
            Task<int> t = Task.Run(() => Person.getAge(BirthDate));
            int age = await t;
            Task<bool> t1 = Task.Run(() => Person.CalculateIsAdult(age));
            Task<string> t2 = Task.Run(() => Person.CalculateSunSign(BirthDate));
            Task<string> t3 = Task.Run(() => Person.CalculateChineseSign(BirthDate));
            Task<bool> t4 = Task.Run(() => Person.CalculateIsBirthday(BirthDate));
            isAdult = await t1;
            sunSign = await t2;
            chineseSign = await t3;
            isBirthday = await t4;
            try
            {
                ourPerson = new Person(FirstName, LastName, Email, BirthDate,isAdult,sunSign,chineseSign,isBirthday,age);
            }
            catch (IncorrectEmailException ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return;
            }
            catch (AgeIsTooBigException ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return;
            }
            catch (BirthdayInFutureException ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return;
            }
            InfoViewModel.AddOnePerson(new RedactorViewModel(ourPerson,gotoInfo));
            await PersonFileRepository.AddToRepositoryOrUpdateAsync(ourPerson);
            gotoInfo.Invoke();
        }
        private bool CanExecute(object o)
        {
            return !String.IsNullOrWhiteSpace(FirstName) && !String.IsNullOrWhiteSpace(LastName) && !String.IsNullOrWhiteSpace(Email);
        }
    }
}
