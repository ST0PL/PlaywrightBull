using Microsoft.Playwright;

namespace PlaywrightBull.Bullmc
{
    internal class BullmcClient : IAsyncDisposable
    {
        private const string _headUrl = "https://bullmc.net/";

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IReadOnlyList<ILocator> _navItems = null!;

        private readonly IPage _page;

        public Uri HeadUrl { get; }

        public string CurrentURL => _page.Url;
        private BullmcClient(IPlaywright playwright, IBrowser browser, IPage page)
        {
            _playwright = playwright;
            _browser = browser;
            _page = page;
            HeadUrl = new(_headUrl);
        }

        public async Task InitAsync()
        {
            string navSelector = ".flex.gap-4.items-center.flex-wrap > button";
            _navItems = await _page.Locator(navSelector).AllAsync();
        }

        public ILocator Locator(string selector, PageLocatorOptions? options = default)
            => _page.Locator(selector, options);

        public Task GotoAsync(string url, PageGotoOptions? options = default)
        {
            UriBuilder uriBuilder = new(url);

            if (HeadUrl.Host != uriBuilder.Host)
                throw new InvalidOperationException($"The host url must be \"{HeadUrl.Host}\".");

            return _page.GotoAsync(url, options);
        }

        public Task GotoIfUrlNotEqualAsync(string url, PageGotoOptions? options = default)
        {
            if (CurrentURL == url)
                return Task.CompletedTask;

            return GotoAsync(url, options);
        }

        public Task GotoMainAsync()
            => GotoIfUrlNotEqualAsync(_headUrl);

        public Task PressAsync(string key, KeyboardPressOptions? options = default)
            => _page.Keyboard.PressAsync(key, options);

        public Task ClickNavButton(int index, LocatorClickOptions? options = null)
            => _navItems[index].ClickAsync(options);

        public static async Task<BullmcClient> CreateAsync(BrowserTypeLaunchOptions? browserOptions = default)
        {
            IPlaywright? playwright = null;
            IBrowser? browser = null;
            IPage page;

            try
            {
                playwright = await Playwright.CreateAsync();
                browser = await playwright.Chromium.LaunchAsync(browserOptions);
                page = await browser.NewPageAsync();
                await page.GotoAsync(_headUrl);
            }
            catch
            {
                playwright?.Dispose();
                if(browser is not null)
                    await browser.DisposeAsync();
                throw;
            }

            return new(playwright, browser, page);
        }
        public async ValueTask DisposeAsync()
        {
            if (_browser == null || _playwright == null)
                return;

            _playwright.Dispose();
            _playwright = null;

            await _browser.DisposeAsync();
            _browser = null;
        }
    }
}
