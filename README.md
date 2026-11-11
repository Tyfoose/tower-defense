# Tower Defense (Godot 4.7, C#)

Лабораторная работа 3. Прототип Tower Defense на компонентах-узлах.

## Управление
- WASD - движение, мышь - прицеливание
- ЛКМ или пробел - стрельба
- E - показать призрак башни, повторно E - поставить
- Q - сменить тип башни, C - отмена
- R - перезапуск после конца игры

## Структура сцен
- GameField - главная сцена: пол, стены, камера, ядро, игрок, башня, спавнер, HUD
- Core - ядро базы (синее и белое), имеет HealthComponent
- Player - игрок (зеленый)
- BasicTower, ExplosionTower, FreezeTower - три типа башен
- Crawler - ползун, бьет при контакте
- EnemyTurret - вражеская стреляющая турель
- PlayerBullet, ExplosionBullet, FreezeBullet - пули игрока и башен
- EnemyBullet - пуля врага

## Компоненты
- HealthComponent - здоровье, смерть, награда металлом
- MoveComponent - движение родителя, замедление
- ShootComponent - создание пуль с перезарядкой
- VisibilityComponent - зона обзора, ближайшая цель
- DamagingComponent - общая логика урона (Area3D), стоит на пулях и на Crawler
- ExplosionComponent - наследник DamagingComponent, расширяет зону и бьет всех в радиусе
- FreezeComponent - наследник DamagingComponent, замедляет цель через MoveComponent
- TowerPlacer - постановка башен игроком через призрак

## Остальные классы
- GameManager - металл, учет башен и врагов, конец игры
- Hud - перезапуск игры
- EnemySpawner - появление врагов
- Player, Tower, Bullet, Crawler, EnemyTurret - скрипты объектов

## Группы
Player, Tower, Core, Enemy, PlayerBullet, EnemyBullet

## Слои физики
1 World, 2 Player, 3 Towers, 4 Core, 5 Enemies