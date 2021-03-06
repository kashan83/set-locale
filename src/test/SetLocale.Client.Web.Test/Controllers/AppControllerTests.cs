﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using Moq;
using NUnit.Framework;

using SetLocale.Client.Web.Entities;
using SetLocale.Client.Web.Services;
using SetLocale.Client.Web.Models;
using SetLocale.Client.Web.Test.TestHelpers;
using SetLocale.Client.Web.Test.Builders;

namespace SetLocale.Client.Web.Test.Controllers
{
    [TestFixture]
    class AppControllerTests
    {
        const string ActionNameDetail = "Detail";
        const string ActionNameNew = "New";

        [Test]
        public async void detail_id_is_greater_than_zero_should_return_app_model()
        {
            //arrange           
            var appService = new Mock<IAppService>();
            appService.Setup(x => x.Get(1)).Returns(() => Task.FromResult(new App{ Id=1, Tokens = new List<Token>(), Url = "url"} ));

            //act
            var sut = new AppControllerBuilder().WithAppService(appService.Object)
                                                .Build();

            var view = await sut.Detail(1) as ViewResult;

           //assert
            Assert.NotNull(view);
            Assert.NotNull(view.Model);

            sut.AssertGetAttribute(ActionNameDetail, new[] { typeof(int) });
            appService.Verify(x => x.Get(1), Times.Once);
        }

        [Test]
        public async void detail_id_is_lesser_than_one_should_redirect_to_home_index()
        {
            //arrange           
            var appService = new Mock<IAppService>();   

            //act 
            var sut = new AppControllerBuilder().WithAppService(appService.Object)
                                                .Build();

            var view = await sut.Detail(0) as RedirectResult;

           //assert
            Assert.NotNull(view); 
            Assert.AreEqual(view.Url, "/");
            
            sut.AssertGetAttribute(ActionNameDetail, new[] { typeof(int) });              
        }
        
        [Test]
        public void new_should_return_app_model()
        {  
            //act
            var sut = new AppControllerBuilder().Build();

            var view = sut.New();

           //assert
            Assert.NotNull(view);
            sut.AssertGetAttribute(ActionNameNew); 
        }
          
        [Test]
        public void new_should_return_app_model_if_model_is_invalid()
        {
            //arrange
            var appService = new Mock<IAppService>();
            var inValidModel = new AppModel { Name = "test name", Url = "test.com" };

            //act
            
            var sut = new AppControllerBuilder().WithAppService(appService.Object)
                                                  .Build();

            var view = sut.New(inValidModel).Result as ViewResult; 

           //assert
            Assert.NotNull(view);
            Assert.NotNull(view.Model);
            var model = view.Model as AppModel;

            Assert.NotNull(model);

            sut.AssertPostAndAntiForgeryTokenAttribute(ActionNameNew, new[] { typeof(AppModel) });
        }
    }
     
}
