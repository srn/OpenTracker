# --------------------------------------------------------
# Host:                         127.0.0.1
# Server version:               5.5.10
# Server OS:                    Win64
# HeidiSQL version:             6.0.0.3853
# Date/time:                    2011-06-09 20:53:20
# --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

# Dumping database structure for opentracker
CREATE DATABASE IF NOT EXISTS `opentracker` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `opentracker`;


# Dumping structure for table opentracker.categories
CREATE TABLE IF NOT EXISTS `categories` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `name` varchar(30) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `image` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL,
  `cat_desc` varchar(255) CHARACTER SET utf8 COLLATE utf8_unicode_ci NOT NULL DEFAULT 'No Description',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.comments
CREATE TABLE IF NOT EXISTS `comments` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `userid` int(10) unsigned NOT NULL DEFAULT '0',
  `torrentid` int(10) unsigned NOT NULL DEFAULT '0',
  `added` int(11) NOT NULL,
  `comment` text COLLATE utf8_unicode_ci NOT NULL,
  `original_text` text COLLATE utf8_unicode_ci NOT NULL,
  `editedby` int(10) unsigned NOT NULL DEFAULT '0',
  `editedate` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userid` (`userid`),
  KEY `torrentid` (`torrentid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

# Data exporting was unselected.


# Dumping structure for table opentracker.forum
CREATE TABLE IF NOT EXISTS `forum` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `categoryid` int(10) unsigned NOT NULL DEFAULT '0',
  `title` varchar(255) NOT NULL,
  `description` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.forum_category
CREATE TABLE IF NOT EXISTS `forum_category` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `title` varchar(255) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.forum_posts
CREATE TABLE IF NOT EXISTS `forum_posts` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `topicid` int(10) unsigned NOT NULL DEFAULT '0',
  `userid` int(10) unsigned NOT NULL DEFAULT '0',
  `content` longtext NOT NULL,
  `added` int(11) unsigned NOT NULL,
  `edited_by` int(11) unsigned NOT NULL,
  `edited_time` int(11) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.forum_topic
CREATE TABLE IF NOT EXISTS `forum_topic` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `forumid` int(10) unsigned NOT NULL DEFAULT '0',
  `userid` int(10) unsigned NOT NULL DEFAULT '0',
  `sticky` int(10) unsigned NOT NULL DEFAULT '0',
  `announcement` int(10) unsigned NOT NULL DEFAULT '0',
  `title` varchar(100) NOT NULL,
  `added` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.peers
CREATE TABLE IF NOT EXISTS `peers` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `torrentid` int(10) unsigned NOT NULL,
  `peer_id` varchar(255) NOT NULL,
  `ip` varchar(64) NOT NULL,
  `port` smallint(5) unsigned NOT NULL,
  `uploaded` bigint(20) unsigned NOT NULL DEFAULT '1',
  `downloaded` bigint(20) unsigned NOT NULL DEFAULT '1',
  `left` bigint(20) unsigned NOT NULL DEFAULT '0',
  `seeding` int(10) unsigned NOT NULL DEFAULT '0',
  `userid` int(10) unsigned NOT NULL,
  `passkey` varchar(32) DEFAULT NULL,
  `connectable` int(10) unsigned NOT NULL DEFAULT '0',
  `useragent` varchar(255) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `userid` (`userid`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.torrents
CREATE TABLE IF NOT EXISTS `torrents` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `categoryid` int(10) unsigned NOT NULL DEFAULT '0',
  `info_hash` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `torrentname` varchar(255) COLLATE utf8_unicode_ci NOT NULL,
  `description` text COLLATE utf8_unicode_ci NOT NULL,
  `description_small` text COLLATE utf8_unicode_ci NOT NULL,
  `added` int(11) NOT NULL,
  `size` bigint(20) unsigned NOT NULL DEFAULT '0',
  `numfiles` int(10) unsigned NOT NULL DEFAULT '0',
  `views` int(10) unsigned NOT NULL DEFAULT '0',
  `snatches` int(10) unsigned NOT NULL DEFAULT '0',
  `last_action` int(11) DEFAULT NULL,
  `visible` enum('yes','no') COLLATE utf8_unicode_ci DEFAULT 'yes',
  `banned` enum('yes','no') COLLATE utf8_unicode_ci DEFAULT 'no',
  `owner` int(10) unsigned DEFAULT '0',
  `client_created_by` char(50) COLLATE utf8_unicode_ci DEFAULT 'unknown' COMMENT 'Retrieves the torrent clients user agent',
  PRIMARY KEY (`id`),
  UNIQUE KEY `info_hash` (`info_hash`),
  KEY `owner` (`owner`),
  KEY `visible` (`visible`),
  KEY `category_visible` (`categoryid`,`visible`),
  FULLTEXT KEY `ft_search` (`description_small`)
) ENGINE=MyISAM DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

# Data exporting was unselected.


# Dumping structure for table opentracker.torrents_files
CREATE TABLE IF NOT EXISTS `torrents_files` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `torrentid` int(10) unsigned NOT NULL DEFAULT '0',
  `filename` varchar(255) NOT NULL,
  `filesize` bigint(20) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.


# Dumping structure for table opentracker.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `passkey` varchar(32) NOT NULL,
  `activatesecret` varchar(32) NOT NULL,
  `username` varchar(50) NOT NULL,
  `passhash` longtext NOT NULL,
  `email` varchar(255) NOT NULL,
  `activated` int(10) unsigned NOT NULL DEFAULT '0',
  `class` int(10) unsigned NOT NULL DEFAULT '1',
  `banned` int(10) unsigned NOT NULL,
  `uploaded` bigint(20) unsigned NOT NULL DEFAULT '10737418240',
  `downloaded` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `passkey` (`passkey`),
  KEY `uploaded` (`uploaded`),
  KEY `downloaded` (`downloaded`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

# Data exporting was unselected.
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
