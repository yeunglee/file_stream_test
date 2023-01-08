using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        const string FILE_PATH = "test_data";

        // SOLUTION #1: change buffer size. for example: 1024, 4096, 1024 * 512 - 1, 1024 * 512 + 1
        const int FILE_BUFFER_SIZE = 1024 * 512;

        Console.WriteLine($".Net version: {Environment.Version}");
        Console.WriteLine($"OS version: {Environment.OSVersion}");

        var fileData = await File.ReadAllBytesAsync(FILE_PATH);
        // SOLUTION #2: add FileOptions.RandomAccess
        await using var fs = new FileStream(FILE_PATH, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, FILE_BUFFER_SIZE, FileOptions.Asynchronous);

        void SetPosition(long p) => fs.Position = p;

        async Task ReadAsync(int size)
        {
            var initialPosition = fs.Position;
            var content = new byte[size];

            // SOLUTION #3: use sync Read instead of async ReadAsync
            var count = await fs.ReadAsync(content, 0, content.Length);

            var isMatched = fileData.Skip((int)initialPosition).Take(count).SequenceEqual(content.Take(count));

            if (!isMatched)
            {
                Console.WriteLine($"!!!Content is inconsistent!!! position={initialPosition:#,0} size={size:#,0} count={count:#,0}");
                Environment.Exit(1);
            }
        }

        await ReadAsync(1);
        await ReadAsync(4);
        await ReadAsync(4);
        await ReadAsync(118);
        await ReadAsync(4);
        await ReadAsync(524288);
        SetPosition(524377);
        await ReadAsync(524288);
        SetPosition(1048633);
        await ReadAsync(524288);
        SetPosition(1572864);
        await ReadAsync(524288);
        await ReadAsync(524288);
        SetPosition(2621431);
        await ReadAsync(524288);

        Console.WriteLine("FileStream is good");
    }
}
