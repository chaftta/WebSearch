using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace WebSearch {
	/// <summary>Command handler</summary>
	internal sealed class SearchCommand {
		/// <summary>Command ID.</summary>
		public const int CommandId = 0x0100;
		/// <summary>Command menu group (command set GUID).</summary>
		public static readonly Guid CommandSet = new Guid("d2a9f75f-4b19-4ea2-b294-2f974357cf37");
		private const string GoogleSearchUrl = "https://www.google.com/search?q=";
		/// <summary>親パッケージ</summary>
		private readonly AsyncPackage package;
		/// <summary>現在のエディタ</summary>
		private EnvDTE80.DTE2 DTE;
		/// <summary>コンストラクタ</summary>
		/// <param name="package">親パッケージ</param>
		/// <param name="commandService">起動サービス.</param>
		private SearchCommand(AsyncPackage package, OleMenuCommandService commandService) {
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(this.Execute, menuCommandID);
			commandService.AddCommand(menuItem);
		}

		/// <summary>Gets the instance of the command.</summary>
		public static SearchCommand Instance { get; private set; }

		/// <summary>コマンドの実行</summary>
		private void Execute(object sender, EventArgs e) {
			ThreadHelper.ThrowIfNotOnUIThread();
			// 現在のエディタを取得
			var doc = DTE.ActiveDocument;
			if (DTE == null) return;
			// 現在のエディタで選択されているテキストを取得
			var selection = (EnvDTE.TextSelection)doc.Selection;
			string selectedText = selection.Text;

			// 選択されたテキストをURLエンコード
			string encodedText = Uri.EscapeDataString(selectedText);

			// デフォルトのブラウザでGoogle検索を開く
			System.Diagnostics.Process.Start(GoogleSearchUrl + encodedText);
		}

		/// <summary>コマンドのエントリー</summary>
		/// <param name="package">親パッケージ</param>
		public static async Task InitializeAsync(AsyncPackage package) {
			// UIスレッドでサービスを取得
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
			var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			// コマンドの作成
			Instance = new SearchCommand(package, commandService);
			Instance.DTE = await Instance.package.GetServiceAsync(typeof(EnvDTE.DTE)) as DTE2;
		}

	}
}
