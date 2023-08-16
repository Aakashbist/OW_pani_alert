using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;

namespace HT.WaterAlerts.Test
{
    public class AutoMoqDataAttribute:AutoDataAttribute
    {
        public AutoMoqDataAttribute():base(() =>
        {
            var fixture = new Fixture();
            fixture.Customize<ContactHistory>(composer => composer.With(p => p.Content, "long content"));
            fixture.Customize<AlertLevel>(composer => composer.With(p => p.Description, "long description"));
            fixture.Customize<Template>(composer => composer
                                                    .With(p => p.SMS, "long sms")
                                                    .With(p => p.Email, "long email")
                                                    .With(p => p.TextToVoice, "long text to voice"));
            fixture.Customize(new AutoMoqCustomization() { ConfigureMembers = true });

            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            return fixture;
        }){}
        
    }
}
