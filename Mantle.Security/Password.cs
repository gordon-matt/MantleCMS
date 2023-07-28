using System.Security.Cryptography;

namespace Mantle.Security;

// http://stackoverflow.com/questions/38995379/alternative-to-system-web-security-membership-generatepassword-in-aspnetcore-ne
public static class Password
{
    private static readonly char[] Punctuations = "!@#$%^&*()_-+=[{]};:>|./?".ToCharArray();

    public static string Generate(int length, int numberOfNonAlphanumericCharacters)
    {
        if (length is < 1 or > 128)
        {
            throw new ArgumentException(nameof(length));
        }

        if (numberOfNonAlphanumericCharacters > length || numberOfNonAlphanumericCharacters < 0)
        {
            throw new ArgumentException(nameof(numberOfNonAlphanumericCharacters));
        }

        using var rng = RandomNumberGenerator.Create();
        byte[] byteBuffer = new byte[length];

        rng.GetBytes(byteBuffer);

        int count = 0;
        char[] characterBuffer = new char[length];

        for (int iter = 0; iter < length; iter++)
        {
            int i = byteBuffer[iter] % 87;

            if (i < 10)
            {
                characterBuffer[iter] = (char)('0' + i);
            }
            else if (i < 36)
            {
                characterBuffer[iter] = (char)('A' + i - 10);
            }
            else if (i < 62)
            {
                characterBuffer[iter] = (char)('a' + i - 36);
            }
            else
            {
                characterBuffer[iter] = Punctuations[i - 62];
                count++;
            }
        }

        if (count >= numberOfNonAlphanumericCharacters)
        {
            return new string(characterBuffer);
        }

        int j;
        var rand = new Random();

        for (j = 0; j < numberOfNonAlphanumericCharacters - count; j++)
        {
            int k;
            do
            {
                k = rand.Next(0, length);
            }
            while (!char.IsLetterOrDigit(characterBuffer[k]));

            characterBuffer[k] = Punctuations[rand.Next(0, Punctuations.Length)];
        }

        return new string(characterBuffer);
    }
}