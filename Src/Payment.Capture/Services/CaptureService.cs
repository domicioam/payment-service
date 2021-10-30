using System;
using Payment.Capture.Repository;

namespace Payment.Capture.Services
{
    public class CaptureService : CanVerifyCapture
    {
        public CaptureService(CaptureRepository captureRepository)
        {
            throw new NotImplementedException();
        }

        public bool CanExecuteCapture(Guid authorisationId)
        {
            //TODO: concurrency problem between the time of the verification and the event registration (must be atomic)
            throw new NotImplementedException();
        }
    }
}