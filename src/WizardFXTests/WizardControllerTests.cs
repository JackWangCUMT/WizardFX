﻿using System;
using NUnit.Framework;
using WizardFX;
using WizardFXTests.MockFactories;

namespace WizardFXTests
{
    [TestFixture]
    class WizardControllerTests
    {
        [Test]
        public void wizard_controller_is_set_when_starting()
        {
            var step = new WizardStep();

            var wizard = new Wizard()
                                    .AddStep(step);

            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            controller.Start(wizard);
            Assert.AreEqual(controller, wizard.Controller);
        }


        [Test]
        public void controller_sends_wizard_to_view_on_move_next()
        {
            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            var step1 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var step2 = new MockStepFactory().AStep.Stub();

            var wizard = new Wizard()
                                    .AddStep(step1)
                                    .AddStep(step2);


            controller.Start(wizard);
            view.Raise(v => v.MovedNext += null, EventArgs.Empty);
            view.Verify(v => v.ShowStep(wizard.CurrentStep));
        }


        [Test]
        public void controller_sends_wizard_to_view_on_move_previous()
        {
            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            var step1 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var step2 = new MockStepFactory().AStep.Stub();

            var wizard = new Wizard()
                                    .AddStep(step1)
                                    .AddStep(step2);

            controller.Start(wizard);
            wizard.JumpToStep(2);
            view.Raise(v => v.MovedPrevious += null, EventArgs.Empty);

            view.Verify(v => v.ShowStep(wizard.CurrentStep));
        }


        [Test]
        public void controller_does_not_send_wizard_to_view_on_finish()
        {
            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            var step1 = new WizardStep(); 
            var wizard = new Wizard()
                    .AddStep(step1);

            controller.Start(wizard);
            view.Raise(v => v.MovedNext += null, EventArgs.Empty);

            view.Verify(v => v.ShowStep(step1), Moq.Times.Once());
        }



        [Test]
        public void on_sub_wizard_completion_previous_wizard_resumes()
        {
            var step1 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var step2 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var step3 = new MockStepFactory().AStep.Stub();
            var mainWizard = new Wizard()
                                    .AddStep(step1)
                                    .AddStep(step2)
                                    .AddStep(step3);

            var step1_1 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var subWizard = new Wizard()
                                    .AddStep(step1_1);

            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            controller.Start(mainWizard);
            view.Raise(v => v.MovedNext += null, EventArgs.Empty);
            Assert.AreEqual(step2, controller.CurrentStep, "At Step 2 before starting Sub Wizard");

            controller.Start(subWizard);
            Assert.AreEqual(step1_1, controller.CurrentStep, "In Sub Wizard after starting Sub Wizard");

            view.Raise(v => v.MovedNext += null, EventArgs.Empty);
            Assert.AreEqual(step2, controller.CurrentStep, "At Step 2 after ending Sub Wizard");
        }

        [Test]
        public void on_sub_wizard_cancel_previous_wizard_resumes()
        {
            var step1 = new MockStepFactory().AStep.ThatCanMoveNext.Stub();
            var step2 = new MockStepFactory().AStep.Stub();
            var step3 = new MockStepFactory().AStep.Stub();
            var mainWizard = new Wizard()
                                    .AddStep(step1)
                                    .AddStep(step2)
                                    .AddStep(step3);

            var step1_1 = new MockStepFactory().AStep.Stub();
            var subWizard = new Wizard()
                                    .AddStep(step1_1);

            var view = new MockViewFactory().AView.Mock();
            var controller = new WizardController(view.Object);

            controller.Start(mainWizard);
            view.Raise(v => v.MovedNext += null, EventArgs.Empty);
            Assert.AreEqual(step2, controller.CurrentStep, "At Step 2 before starting Sub Wizard");

            controller.Start(subWizard);
            Assert.AreEqual(step1_1, controller.CurrentStep, "In Sub Wizard after starting Sub Wizard");

            view.Raise(v => v.Cancelled += null, EventArgs.Empty);
            Assert.AreEqual(step2, controller.CurrentStep, "At Step 2 after cancel Sub Wizard");
        }

    }
}