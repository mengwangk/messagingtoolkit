<!DOCTYPE html><html><head><title>Skype</title><meta charset="utf-8"/><meta http-equiv="X-UA-Compatible" content="IE=edge"/><link rel="stylesheet" href="css/login.css?b7317e0f670339819e857f5d40b3e721" type="text/css"/><script src="js/login.js?3a5cffa37a220fbbdec469d128a5e2e6"></script><script src="languages/en.built.js?42c4872c61e4a0005777b8ef767c4fc0" charset="utf-8" id="languageTag"></script></head><body><script>
        /*
         Due to race conditions, we must wait for the window.external API to be initialized.
         Please see #AABIG-3214 for more details.
         */
        var CL_init = function() {
            // Initialise BrowserAPI
            window.API = new SKYPE.login.Api();
            API.log("Hello from ClientLogin v." + Config.buildVersion + ", API initialized.");

            window.HighContrast = new SKYPE.login.HighContrast();

            window.EcsConfig = new SKYPE.login.ecs.Config();
            EcsConfig.loadDefaultValues();

            window.Login = {
                visitor: new SKYPE.login.Visitor()
            };

            window.EcsClient = new SKYPE.login.ecs.Client();
            EcsClient.init();

            // Set language on Skype start. There is a similar code in Translate.js.
            // But this code works only after DOM is loaded. It is used when the user changes the language
            // in the Skype client menu (available on Windows only).
            // Using document.write() makes the translations ready immediately. Avoids blink from default English to the target
            // language on DOM load.
            if (API.getLanguage() != "en") {
                document.write("<script src='languages/" + API.getLanguage().toLowerCase() + ".built.js'><\/script>");
            }

            $(function() {
                API.log("Document loaded, initializing ClientLogin");
                window.HighContrast.start();
                new SKYPE.login.Main();
            });
        };

        var CL_testLoad = function() {
            try {
                // This throws exception if BrowserAPI is not ready
                getExternal();
            } catch (e) {
                // Try again in 100 ms
                return;
            }

            // BrowserAPI is ready. Clear this loader and continue with the
            // standard ClientLogin init.
            window.clearInterval(window.CL_loader);
            CL_init();
        };

        window.CL_loader = window.setInterval(CL_testLoad, 100);
        CL_testLoad(); // call tester immediately rather than waiting for the interval
    </script><div class="view v2 noflip" id="unifiedSkypeLoginView"><span class="logo"></span><div id="pageTitleOrErrorMsgContainer"><h2 id="unifiedLoginHeader" tabindex="2" data-translation-key="signInSubtitleWithYourMSA" class="signInTitle">Sign in <span class="subtitle">with your Microsoft account</span></h2><div id="unifiedLoginError" role="alert" aria-live="assertive" display="none"><div class="message" id="unifiedMessageErrorSkypeLogin" role="alert" tabindex="1"><span class="messageBody"></span></div></div></div><form action="#" id="unifiedForm"><div id="unifiedUsernameContainer" class="unifiedUsernameContainer"><input type="text" id="unifiedUsername" maxlength="150" tabindex="3" role="textbox" placeholder="" aria-labelledby="usernameSimulatedPlaceholder"/><div id="usernameSimulatedPlaceholder" class="simulatedPlaceholder" data-translation-key="skypeNameEmailMobile">Skype name, email or phone</div></div><div id="unifiedPasswordContainer" class="unifiedPasswordContainer"><input type="password" id="unifiedPassword" maxlength="256" tabindex="5" placeholder="" aria-labelledby="passwordSimulatedPlaceholder"/><div id="passwordSimulatedPlaceholder" class="simulatedPlaceholder" data-translation-key="password">Password</div></div><div id="unifiedSignInContainer" class="signInContainer"><a href="#" id="unifiedSignIn" class="blueRoundButton darker signInDisabled" tabindex="6" role="button" data-translate-attribute="aria-label" data-translation-key="signInButton" aria-label="Sign in"><span class="leftBookend"><span class="rightBookend" data-translation-key="signInButton">Sign in</span></span></a><div class="newAccountContainer"><a href="#" id="unifiedDefaultCreateAccount" tabindex="9" data-translation-key="createAccount">Create new account</a></div></div><div id="unifiedLoadingSpinner"><p class="signInText" data-translation-key="signingIn" aria-role="hidden">Signing in...</p></div></form><div class="unifiedLoginFooter"><a href="#" id="unifiedProblemsSigningIn" class="unifiedProblemsSigningIn" target="_blank" tabindex="11" data-translation-key="problemsSigningIn" data-track-action="ClickProblemsSigningIn">Problem signing in?</a><div class="facebookLoginContainer"><div id="unifiedFacebookSignInIcon"></div><a href="#" id="unifiedFacebookSignIn" class="unifiedFacebookSignIn" tabindex="10" data-translation-key="facebookLogin">Sign in with Facebook</a></div></div></div><div class="view v2 noflip" id="disambiguationView"><a href="#" class="backLink" id="backLinkDisambiguationView" tabindex="50"><span data-translation-key="back">Back</span></a><div class="centred"><h1 id="disambiguationYourAccountsHeader" data-translation-key="yourAccounts" tabindex="1">Your accounts</h1><h2 id="disambiguationSelectAccountHeader" data-translation-key="txtSelectAccountToUse" tabindex="2">Select the account you'd like to use.</h2><div id="disambiguationAccountsContainer"></div></div></div><div class="view" id="createView"><div class="contentCenter"><div class="pad"></div><div class="columnCenter"><h2 class="titleLarge" data-translation-key="acceptTou" tabindex="1">Please read and accept Terms of Use</h2><div class="touWrap pixelpad"><p class="paragraph" data-translation-key="likeToKnow" tabindex="2">Inform me about new products, features and special offers.</p></div><div class="touWrap pixelpad"><a href="#" id="allowSpam" class="checkbox" role="checkbox" data-translation-key="byEmail" tabindex="3">By email</a></div><div class="touWrap pixelpad"><a href="#" id="allowSms" class="checkbox" role="checkbox" data-translation-key="bySms" tabindex="4">By SMS (Your operator may charge to receive messages)</a></div><div class="pixelpad"><p id="confirmTou" class="paragraph" data-translation-key="confirmTou"> Yes, I have read and I accept the <a href="#" id="touLink" tabindex="5" data-track-action="ClickTermsOfUse">Skype Terms of Use</a> and the <a href="#" id="privacyLink" tabindex="6" data-track-action="ClickPrivacy">Skype Privacy Statement</a>. </p></div><div class="margin-top"><a href="#" id="createTechnical" class="button" tabindex="7"><span data-translation-key="agreeJoin">I agree - join Skype</span></a><a href="#" class="backLink" id="backLinkCreate" tabindex="8"><span data-translation-key="back">Back</span></a></div></div></div></div><div class="view v2" id="loadingView"><span class="logo"><img src="images/normal/skype-logo-136x60.png" alt="Skype" class="skypeLogo"/></span><p class="signInText" data-translation-key="signingIn" aria-role="hidden">Signing in...</p><div aria-role="alert" style="display:block"><p class="screenReaderSignInText" aria-role="alert" tabindex="1" data-translation-key="signingInPleaseWait">Signing in to Skype, please wait</p></div></div></body></html>                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      � ��                                                                     