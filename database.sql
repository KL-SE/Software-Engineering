-- phpMyAdmin SQL Dump
-- version 4.7.4
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Oct 23, 2017 at 08:06 PM
-- Server version: 10.1.26-MariaDB
-- PHP Version: 7.1.9

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `Software_Engineering`
--

-- --------------------------------------------------------

--
-- Table structure for table `appointments`
--

CREATE TABLE `appointments` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `medical_staff_id` int(11) NOT NULL,
    `patient_id` int(11) NOT NULL,
    `date_appointed` datetime NOT NULL,
    `cancelled` tinyint(1) NOT NULL DEFAULT 0,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `contact_numbers`
--

CREATE TABLE `contact_numbers` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `number` varchar(24) NOT NULL COLLATE UTF8_GENERAL_CI,
    `personal_details_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `identifications`
--

CREATE TABLE `identifications` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `value` varchar(24) NOT NULL COLLATE UTF8_GENERAL_CI,
    `personal_details_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `personal_details`
--

CREATE TABLE `personal_details` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `first_name` varchar(30) NOT NULL COLLATE UTF8_GENERAL_CI,
    `last_name` varchar(30) NOT NULL COLLATE UTF8_GENERAL_CI,
    `address` varchar(30) NOT NULL COLLATE UTF8_GENERAL_CI,
    `postcode_id` int(11) NOT NULL,
	`date_of_birth` datetime NOT NULL,
	`sex` char(1) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `postal_codes`
--

CREATE TABLE `postal_codes` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `code` varchar(24) NOT NULL COLLATE UTF8_GENERAL_CI,
    `city_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `cities`
--

CREATE TABLE `cities` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `state_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `states`
--

CREATE TABLE `states` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `country_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `countries`
--

CREATE TABLE `countries` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `staffs`
--

CREATE TABLE `staffs` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `type` char(1) NOT NULL DEFAULT 'M', -- M for Medical Staff, R for Receptionist
	`personal_details_id` int(11) NOT NULL,
    `password` varchar(100) NOT NULL,
    `date_joined` date NOT NULL,
    `active` tinyint(1) NOT NULL DEFAULT 1,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `medical_staffs`
--

CREATE TABLE `medical_staffs` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `staff_id` int(11) NOT NULL,
    `licence_no` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `receptionists`
--

CREATE TABLE `receptionists` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `staff_id` int(11) NOT NULL,
    `admin` tinyint(1) NOT NULL DEFAULT '1',
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `medications`
--

CREATE TABLE `medications` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `code` varchar(11) NOT NULL UNIQUE COLLATE UTF8_GENERAL_CI,
    `name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `patients`
--

CREATE TABLE `patients` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `personal_details_id` int(1) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `prescriptions`
--

CREATE TABLE `prescriptions` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `prescriber_id` int(11) NOT NULL,
    `patient_id` int(11) NOT NULL,
    `name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
    `start_date` date NOT NULL,
    `end_date` date NOT NULL,
    `remark` text NOT NULL DEFAULT '' COLLATE UTF8_GENERAL_CI,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `prescriptionMedications`
--

CREATE TABLE `prescription_medications` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `presciption_id` int(11) NOT NULL,
    `medication_id` int(11) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `testResults`
--

CREATE TABLE `test_results` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `patient_id` int(11) NOT NULL,
	`name` varchar(100) NOT NULL COLLATE UTF8_GENERAL_CI,
	`medical_license_no` varchar(11) NOT NULL COLLATE UTF8_GENERAL_CI,
    `description` text NOT NULL COLLATE UTF8_GENERAL_CI,
    `result` text NOT NULL COLLATE UTF8_GENERAL_CI,
    `remark` text NOT NULL DEFAULT '' COLLATE UTF8_GENERAL_CI,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `workingDays`
--

CREATE TABLE `working_days` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `staff_id` int(11) NOT NULL,
    `day_1` tinyint(1) NOT NULL,
    `day_2` tinyint(1) NOT NULL,
    `day_3` tinyint(1) NOT NULL,
    `day_4` tinyint(1) NOT NULL,
    `day_5` tinyint(1) NOT NULL,
    `day_6` tinyint(1) NOT NULL,
    `day_7` tinyint(1) NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- Table structure for table `leaveDates`
--

CREATE TABLE `leave_dates` (
    `id` int(11) NOT NULL AUTO_INCREMENT,
    `staff_id` int(11) NOT NULL,
    `date` date NOT NULL,
    `created_on` datetime,
    `last_updated` datetime,
    PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

ALTER TABLE appointments ADD FOREIGN KEY(medical_staff_id) REFERENCES staffs(id);
ALTER TABLE appointments ADD FOREIGN KEY(patient_id) REFERENCES patients(id);
ALTER TABLE receptionists ADD FOREIGN KEY(staff_id) REFERENCES staffs(id);
ALTER TABLE medical_staffs ADD FOREIGN KEY(staff_id) REFERENCES staffs(id);
ALTER TABLE prescriptions ADD FOREIGN KEY(prescriber_id) REFERENCES staffs(id);
ALTER TABLE prescriptions ADD FOREIGN KEY(patient_id) REFERENCES patients(id);
ALTER TABLE prescription_medications ADD FOREIGN KEY(presciption_id) REFERENCES prescriptions(id);
ALTER TABLE prescription_medications ADD FOREIGN KEY(medication_id) REFERENCES medications(id);
ALTER TABLE test_results ADD FOREIGN KEY(patient_id) REFERENCES patients(id);
ALTER TABLE working_days ADD FOREIGN KEY(staff_id) REFERENCES staffs(id);
ALTER TABLE leave_dates ADD FOREIGN KEY(staff_id) REFERENCES staffs(id);

ALTER TABLE staffs ADD FOREIGN KEY(personal_details_id) REFERENCES personal_details(id);
ALTER TABLE patients ADD FOREIGN KEY(personal_details_id) REFERENCES personal_details(id);
ALTER TABLE identifications ADD FOREIGN KEY(personal_details_id) REFERENCES personal_details(id);
ALTER TABLE contact_numbers ADD FOREIGN KEY(personal_details_id) REFERENCES personal_details(id);
ALTER TABLE postal_codes ADD FOREIGN KEY(city_id) REFERENCES cities(id);
ALTER TABLE cities ADD FOREIGN KEY(state_id) REFERENCES states(id);
ALTER TABLE states ADD FOREIGN KEY(country_id) REFERENCES countries(id);

--
-- Metadata
--
USE `phpmyadmin`;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
