-- phpMyAdmin SQL Dump
-- version 4.6.4
-- https://www.phpmyadmin.net/
--
-- Client :  127.0.0.1
-- Généré le :  Lun 05 Juin 2017 à 18:40
-- Version du serveur :  5.7.14
-- Version de PHP :  5.6.25

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de données :  `easydbtests`
--

DELIMITER $$
--
-- Procédures
--
DROP PROCEDURE IF EXISTS `get_output_age`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `get_output_age` (IN `p_id` INT, OUT `p_age` INT)  NO SQL
select age into p_age from users where id=p_id$$

DROP PROCEDURE IF EXISTS `get_user`$$
CREATE DEFINER=`root`@`localhost` PROCEDURE `get_user` (IN `p_id` INT)  NO SQL
select * from users where id=p_id$$

--
-- Fonctions
--
DROP FUNCTION IF EXISTS `get_username_function`$$
CREATE DEFINER=`root`@`localhost` FUNCTION `get_username_function` (`p_id` INT) RETURNS VARCHAR(255) CHARSET latin1 NO SQL
begin
DECLARE result varchar(255);
select username into result from users where id=p_id;
return result;
end$$

DELIMITER ;

-- --------------------------------------------------------

--
-- Structure de la table `users`
--

DROP TABLE IF EXISTS `users`;
CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `username` varchar(255) NOT NULL,
  `age` int(11) DEFAULT NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Contenu de la table `users`
--

INSERT INTO `users` (`id`, `username`, `age`) VALUES
(1, 'user1', 20),
(2, 'user2', 30),
(3, 'user3', NULL);

--
-- Index pour les tables exportées
--

--
-- Index pour la table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT pour les tables exportées
--

--
-- AUTO_INCREMENT pour la table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=22;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
