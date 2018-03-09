﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Lands.ViewModels {
    using GalaSoft.MvvmLight.Command;
    using System.ComponentModel;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Views;
    using Lands.Servicios;

    public class LoginViewModel : BaseViewModel {

        #region servicios
        private ApiService apiService;
        #endregion

        #region Atributos
        private string email;
        private string password;
        private bool isRunnig;
        private bool isEnabled;

        #endregion

        #region Propiedades
        public string Email {
            get {
                return this.email;
            }
            set { SetValue(ref this.email, value); }
        }
        public string Password {
            get {
                return this.password;
            }
            set { SetValue(ref this.password, value); }

        }
        public bool IsRunning {

            get {
                return this.isRunnig;
            }
            set { SetValue(ref this.isRunnig, value); }
        }
        public bool IsRemembered { get; set; }
        public bool IsEnabled {
            get {
                return this.isEnabled;
            }
            set { SetValue(ref this.isEnabled, value); }
        }

        #endregion

        #region constructor
        public LoginViewModel() {
            this.apiService = new ApiService();
            this.IsRemembered = true;
            this.IsEnabled = true;
         //   this.Email = "magora84@hotmail.com";
           // this.Password = "1234";
        }
        #endregion
        #region Commandos
        public ICommand LoginCommand {
            get {
                return new RelayCommand(login);
            }
        }



        private async void login() {
            if (string.IsNullOrEmpty(this.Email)) {
                await Application.Current.MainPage.DisplayAlert(
                    "Error"
                    , "Debes introducir tu email"
                    , "Aceptar");
                return;
            }
            if (string.IsNullOrEmpty(this.Password)) {
                await Application.Current.MainPage.DisplayAlert(
                    "Error"
                    , "Debes introducir tu Password"
                    , "Aceptar");
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess) {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "error",
                    connection.Message,
                    "aceptar"
                    );
                this.Password = string.Empty;
                return;
            }
            var token = await this.apiService.GetToken("http://landsapil.azurewebsites.net", this.Email, this.Password);
            if (token == null) {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "error",
                    "algo salio mal please try later",
                    "aceptar"
                    );
                this.Password = string.Empty;
                return;
            }
            if (string.IsNullOrEmpty(token.AccessToken)) {
                this.IsRunning = false;
                this.IsEnabled = true;
                await Application.Current.MainPage.DisplayAlert(
                    "error",
                    token.ErrorDescription,
                    "aceptar"
                    );
                this.Password = string.Empty;
                return;
            }
            /*
                        if (this.Email != "magora84@hotmail.com" || this.Password != "1234") {
                            this.IsRunning = false;
                            this.IsEnabled = true;

                            await Application.Current.MainPage.DisplayAlert(
                                "error",
                                "email o password incorrecto",
                                "aceptar"
                                );
                            this.Password = string.Empty;
                            return;
                        }*/

            this.IsRunning = false;
            this.IsEnabled = true;

            this.Email = string.Empty;
            this.Password = string.Empty;
            MainViewModel.GetInstance().Lands = new LansViewModel();
            await Application.Current.MainPage.Navigation.PushAsync(new LandsPage());
        }
        #endregion

    }
}
