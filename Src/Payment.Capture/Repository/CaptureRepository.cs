using System;

namespace Payment.Capture.Repository
{
    public class CaptureRepository
    {
        public virtual decimal GetAmountLeftToCapture(Guid authorisationId)
        {
            throw new NotImplementedException();
        }
    }
}