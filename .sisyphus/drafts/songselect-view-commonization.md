# Draft: SongSelect View 共通化方針

## Requirements (confirmed)
- 相談対象: `Assets/OctaNotes/Scripts/SongSelect/View` のスクリプト設計
- `GameConfigView` で `PlaySettingsSO` の設定を書き換えるUIを実装予定
- `SongListView` と「5枚カード表示」「Button0で下、Button1で上へ切替」の挙動がほぼ共通
- 相談内容: 共通化するべきか、完全分離するべきか

## Technical Decisions
- 推奨方針（暫定）: **中庸（入力・選択状態遷移・5カード配置ロジックのみ共通化、データ取得と確定時副作用は分離）**
- 理由: 画面操作のUX一貫性は担保しつつ、SongとConfigで将来差分が出やすいドメイン処理を分離できるため

## Research Findings
- Oracleコンサルを2回試行したが、いずれもタイムアウト（外部分析結果は未取得）
- そのため、現時点は一般的なUnity View設計原則に基づく暫定提案で整理

## Open Questions
- 共通部分は「見た目の並び」だけか、「入力処理・選択状態遷移」まで含むか
- 今後、5カードUIを再利用する画面が増える見込みがあるか
- `SongListView` と `GameConfigView` の差分（データ供給方法・決定時副作用）の境界はどこか
- `PlaySettingsSO` 書き換えは「選択中に即時反映」か「決定時のみ反映」か
- アニメーション/演出差分（曲一覧と設定一覧で将来変える予定）があるか

## Scope Boundaries
- INCLUDE: SongSelect配下View層の設計方針
- EXCLUDE: 実装そのもの（コード変更は未実施）
