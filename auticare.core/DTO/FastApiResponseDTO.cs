using auticare.core.DTO;

public class FastApiResponse
{
    public string status { get; set; }
    public AiPredictionResultDto result { get; set; }
}