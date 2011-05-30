# --------------------------------------------------------
# Host:                         127.0.0.1
# Server version:               5.5.10
# Server OS:                    Win64
# HeidiSQL version:             6.0.0.3797
# Date/time:                    2011-04-30 01:05:15
# --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

# Dumping database structure for opentracker
CREATE DATABASE IF NOT EXISTS `opentracker` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `opentracker`;


# Dumping structure for table opentracker.peers
CREATE TABLE IF NOT EXISTS `peers` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `info_hash` varchar(255) NOT NULL,
  `peer_id` varchar(255) NOT NULL,
  `ip` varchar(64) NOT NULL,
  `port` smallint(5) unsigned NOT NULL,
  `uploaded` bigint(20) unsigned NOT NULL DEFAULT '0',
  `downloaded` bigint(20) unsigned NOT NULL DEFAULT '0',
  `left` bigint(20) unsigned NOT NULL DEFAULT '0',
  `update_time` datetime NOT NULL,
  `expire_time` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;

# Dumping data for table opentracker.peers: ~4 rows (approximately)
/*!40000 ALTER TABLE `peers` DISABLE KEYS */;
INSERT INTO `peers` (`id`, `info_hash`, `peer_id`, `ip`, `port`, `uploaded`, `downloaded`, `left`, `update_time`, `expire_time`) VALUES
	(2, 'Pz99P3IlPz8kDmg4Pz90Pz8/', 'LVVUMzAwQi0/Yj8/Pxs/STc/Pw==', '127.0.0.1', 46593, 5627, 0, 0, '2011-04-29 22:14:19', '0001-01-01 00:00:00'),
	(3, 'Pz99P3IlPz8kDmg4Pz90Pz8/', 'TTctMi0xLS1yYjw/P0s/SkI/OUY=', '127.0.0.1', 61048, 0, 0, 0, '2011-04-29 22:19:00', '0001-01-01 00:00:00'),
	(4, 'Py4/JSdsBys/Rz8nPxIwCD8/Pw==', 'LVVUMzAwQi0/Yj8/Pxs/STc/Pw==', '::1', 46593, 71690604, 0, 0, '2011-04-29 22:59:14', '0001-01-01 00:00:00'),
	(5, 'Py4/JSdsBys/Rz8nPxIwCD8/Pw==', 'TTctMi0xLS1yYjw/P0s/SkI/OUY=', '::1', 61048, 0, 35845302, 0, '2011-04-29 22:59:21', '0001-01-01 00:00:00');
/*!40000 ALTER TABLE `peers` ENABLE KEYS */;


# Dumping structure for table opentracker.torrents
CREATE TABLE IF NOT EXISTS `torrents` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `info_hash` varchar(40) COLLATE utf8_unicode_ci NOT NULL,
  `name` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `filename` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `save_as` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `search_text` text COLLATE utf8_unicode_ci NOT NULL,
  `descr` text COLLATE utf8_unicode_ci NOT NULL,
  `ori_descr` text COLLATE utf8_unicode_ci NOT NULL,
  `category` int(10) unsigned NOT NULL DEFAULT '0',
  `size` bigint(20) unsigned NOT NULL DEFAULT '0',
  `added` int(11) NOT NULL,
  `type` enum('single','multi') COLLATE utf8_unicode_ci NOT NULL DEFAULT 'single',
  `numfiles` int(10) unsigned NOT NULL DEFAULT '0',
  `comments` int(10) unsigned NOT NULL DEFAULT '0',
  `views` int(10) unsigned NOT NULL DEFAULT '0',
  `hits` int(10) unsigned NOT NULL DEFAULT '0',
  `times_completed` int(10) unsigned NOT NULL DEFAULT '0',
  `leechers` int(10) unsigned NOT NULL DEFAULT '0',
  `seeders` int(10) unsigned NOT NULL DEFAULT '0',
  `last_action` int(11) NOT NULL,
  `visible` enum('yes','no') COLLATE utf8_unicode_ci NOT NULL DEFAULT 'yes',
  `banned` enum('yes','no') COLLATE utf8_unicode_ci NOT NULL DEFAULT 'no',
  `owner` int(10) unsigned NOT NULL DEFAULT '0',
  `numratings` int(10) unsigned NOT NULL DEFAULT '0',
  `ratingsum` int(10) unsigned NOT NULL DEFAULT '0',
  `nfo` text COLLATE utf8_unicode_ci NOT NULL,
  `client_created_by` char(50) COLLATE utf8_unicode_ci NOT NULL DEFAULT 'unknown',
  PRIMARY KEY (`id`),
  UNIQUE KEY `info_hash` (`info_hash`),
  KEY `owner` (`owner`),
  KEY `visible` (`visible`),
  KEY `category_visible` (`category`,`visible`),
  FULLTEXT KEY `ft_search` (`search_text`,`ori_descr`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

# Dumping data for table opentracker.torrents: 0 rows
/*!40000 ALTER TABLE `torrents` DISABLE KEYS */;
/*!40000 ALTER TABLE `torrents` ENABLE KEYS */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
