using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace WebSearch {
	/// <summary>拡張機能を公開するクラス</summary>
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(WebSearchPackage.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	public sealed class WebSearchPackage : AsyncPackage {
		/// <summary>WebSearchPackage GUID string.</summary>
		public const string PackageGuidString = "286fd970-b62f-4efd-a053-284c67971c0c";
		/// <summary>パッケージの初期化(このメソッドはパッケージが設置された直後に呼び出されるので、初期化をここで行う)</summary>
		/// <param name="cancellationToken">VSのシャットダウン時に発生する初期化キャンセルを監視するキャンセルトークン</param>
		/// <param name="progress">進捗状況を管理するプロバイダー</param>
		/// <returns>パッケージ初期化の非同期作業を表すタスクか終了タスク。(※NULLを返してはいけない)</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress) {
			await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
			await SearchCommand.InitializeAsync(this);
		}
	}
}
